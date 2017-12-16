using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public static class RectangleExtensions
    {
        public static Point Center(this Rectangle rect)  
            => new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

        public static bool IntersectsWith(this Rectangle rect, IEnumerable<Rectangle> otherRects)
            => otherRects.Any(rect.IntersectsWith);
    }
}
