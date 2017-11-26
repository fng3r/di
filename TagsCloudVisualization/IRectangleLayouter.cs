using System.Drawing;

namespace TagsCloudVisualization
{
    public interface IRectangleLayouter
    {
        Rectangle PutNextRectangle(Size rectangleSize);
    }
}