using System.Drawing;

namespace TagsCloudVisualization
{
    public class TagsCloudVisualizerConfiguration
    {
        public readonly Color DefaultBackgroundColor = Color.White;
        public readonly Color DefaultForegroundColor = Color.Blue;
        public readonly int DefaultPenWidth = 2;

        public Color BackgroundColor { get; private set; }
        public Brush Brush { get; private set; }
        public Pen Pen { get; }

        public TagsCloudVisualizerConfiguration()
        {
            BackgroundColor = DefaultBackgroundColor;
            Brush = new SolidBrush(DefaultForegroundColor);
            Pen = new Pen(DefaultForegroundColor, DefaultPenWidth);
        }

        public TagsCloudVisualizerConfiguration SetBackground(Color color)
        {
            BackgroundColor = color;
            return this;
        }

        public TagsCloudVisualizerConfiguration SetForeground(Color color)
        {
            Brush = new SolidBrush(color);
            Pen.Color = color;
            return this;
        }
    }
}