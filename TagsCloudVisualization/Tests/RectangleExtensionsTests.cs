using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    [TestFixture]
    public class RectangleExtensionsTests
    {
        [TestCaseSource(typeof(RectangleTestData), nameof(RectangleTestData.CenterTestData))]
        public Point Center(Rectangle rect)
        {
            return rect.Center();
        }

        [TestCaseSource(typeof(RectangleTestData), nameof(RectangleTestData.IntersectsWithTestData))]
        public bool IntersectsWith(Rectangle rect, IEnumerable<Rectangle> otherRects)
        {
            return rect.IntersectsWith(otherRects);
        }
    }

    public class RectangleTestData
    {
        public static IEnumerable IntersectsWithTestData
        {
            get
            {
                yield return new TestCaseData(
                        new Rectangle(0, 0, 5, 5), new[] { new Rectangle(10, 10, 1, 1), new Rectangle(15, 15, 1, 1) })
                    .Returns(false);
                yield return new TestCaseData(new Rectangle(0, 0, 5, 5), new[] { new Rectangle(0, 0, 5, 5), new Rectangle(100, 100, 5, 5) })
                    .Returns(true);
                yield return new TestCaseData(new Rectangle(0, 0, 5, 5), Array.Empty<Rectangle>()).Returns(false);
            }
        }

        public static IEnumerable CenterTestData
        {
            get
            {
                yield return new TestCaseData(new Rectangle(0, 0, 10, 20)).Returns(new Point(5, 10));
                yield return new TestCaseData(new Rectangle(0, 100, 50, 50)).Returns(new Point(25, 125));
                yield return new TestCaseData(new Rectangle(-100, -50, 200, 50)).Returns(new Point(0, -25));
            }
        }
    }
}