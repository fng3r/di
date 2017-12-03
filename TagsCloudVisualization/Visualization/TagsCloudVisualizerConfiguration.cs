using System.Drawing;

namespace TagsCloudVisualization
{
    public class TagsCloudVisualizerConfiguration
    {
        public readonly Color DefaultBackgroundColor = Color.White;
        public readonly Color DefaultForegroundColor = Color.Blue;
        public readonly int DefaultPenWidth = 2;
        public readonly int DefaultFontSize = 80;
        public readonly string DefaultFontFamily = "Arial";

        public Color BackgroundColor { get; private set; }
        public Brush Brush { get; private set; }
        public Pen Pen { get; }
        public Font Font { get; private set; }

        public TagsCloudVisualizerConfiguration()
        {
            BackgroundColor = DefaultBackgroundColor;
            Brush = new SolidBrush(DefaultForegroundColor);
            Pen = new Pen(DefaultForegroundColor, DefaultPenWidth);
            Font = new Font(DefaultFontFamily, DefaultFontSize);
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

        public TagsCloudVisualizerConfiguration SetFont(Font font)
        {
            Font = font;
            return this;
        }
    }
}