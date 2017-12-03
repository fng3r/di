using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter : IRectangleLayouter
    {
        private readonly IEnumerable<Point> pointsGenerator;
        public List<Rectangle> Rectangles { get; }

        public CircularCloudLayouter(IPointsGenerator pointsGenerator)
        {
            this.pointsGenerator = pointsGenerator;
            Rectangles = new List<Rectangle>();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            foreach (var point in pointsGenerator)
            {
                var rectangleLocation = new Point(point.X - rectangleSize.Width / 2, point.Y - rectangleSize.Height / 2);
                var rectangle = new Rectangle(rectangleLocation, rectangleSize);
                if (rectangle.IntersectsWith(Rectangles)) continue;
                Rectangles.Add(rectangle);

                return rectangle;
            }

            return Rectangle.Empty;
        }
    }
}
