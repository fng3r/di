using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization
{
    public class Spiral : IPointsGenerator
    {
        private readonly Point initialPoint;

        public Spiral(Point initialPoint) => this.initialPoint = initialPoint;

        public IEnumerator<Point> GetEnumerator()
        {
            var t = 0.01;
            yield return initialPoint;
            while (true)
            {
                var x = (int)(initialPoint.X + Math.Exp(0.01 * t) * Math.Cos(t));
                var y = (int)(initialPoint.Y + Math.Exp(0.01 * t) * Math.Sin(t));
                yield return new Point(x, y);
                t += 0.05;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public interface IPointsGenerator : IEnumerable<Point>
    {
    }
}