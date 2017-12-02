using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FluentAssertions;
using MoreLinq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace TagsCloudVisualization
{
    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private Point center;
        private CircularCloudLayouter layouter;

        [SetUp]
        public void SetUp()
        {
            center = new Point(0, 0);
            layouter = new CircularCloudLayouter(new Spiral(center)); 
        }


        [Test]
        public void Place_FirstRectangle_AtCenter()
        {
            layouter.PutNextRectangle(new Size(10, 10)).Center().Should().Be(center);
        }

        [TestCase(0, 0)]
        [TestCase(10, 200)]
        [TestCase(1000, 50)]
        public void PutRectangle_WithGivenSize(int width, int height)
        {
            var rectSize = new Size(width, height);
            layouter.PutNextRectangle(rectSize).Size.Should().Be(rectSize);
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(100)]
        public void Save_AllPlacedRectangles(int count)
        {
            PutRandomRectangles(count);
            layouter.Rectangles.Should().HaveCount(count);
        }

        [Test]
        public void NotContain_IntersectedRectangles()
        {
            PutRandomRectangles(100);
            var rectangles = layouter.Rectangles;
            rectangles.All(rect => rectangles.All(otherRect => otherRect == rect || !rect.IntersectsWith(otherRect))).Should().BeTrue();
        }

        [TestCase(100, 50)]
        [TestCase(50, 500)]
        public void EvenlyDistribute_Rectangles(int count, int maxRectBound)
        {
            PutRandomRectangles(count, maxRectBound);

            var rectangles = layouter.Rectangles;
            var leftmostRect = rectangles.MinBy(r => r.Center().X);
            var rightmostRect = rectangles.MaxBy(r => r.Center().X);
            var uppermostRect = rectangles.MaxBy(r => r.Center().Y);
            var nethermostRect = rectangles.MinBy(r => r.Center().Y);

            var outerRects = new[] { leftmostRect, rightmostRect, uppermostRect, nethermostRect };

            var distancesToCenter = outerRects
                .Select(r => r.Center())
                .Select(p => GetVectorLength(p, center));

            distancesToCenter
                .SelectMany(d1 => distancesToCenter.Select(d2 => Math.Abs(d1 - d2)))
                .Max()
                .Should().BeInRange(0, maxRectBound);

            double GetVectorLength(Point a, Point b)
            {
                return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
            }
        }

        [TearDown]
        public void TearDown()
        {
            var context = TestContext.CurrentContext;
            if (context.Result.Outcome.Status == TestStatus.Failed)
            {
                var outputFile = $@"{context.TestDirectory}\FailedTests\{context.Test.Name}.png";
                var visualizer = new TagsCloudVisualizer(new TagsCloudVisualizerConfiguration());
                var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                var graphics = Graphics.FromImage(bitmap);
                visualizer.DrawRectangles(graphics, layouter.Rectangles);
                bitmap.Save(outputFile);

                Console.WriteLine($"Tag cloud visualization saved to file {outputFile}");
            }
        }


        private void PutRandomRectangles(int count, int maxBound = 100)
        {
            foreach (var size in GetRandomRectangleSizes(count, maxBound))
                layouter.PutNextRectangle(size);
        }

        private IEnumerable<Size> GetRandomRectangleSizes(int count, int maxBound)
        {
            Random rnd = new Random();
            return GenerateSizes().Take(count);

            IEnumerable<Size> GenerateSizes()
            {
                while (true)
                    yield return new Size(rnd.Next(maxBound), rnd.Next(maxBound));
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}