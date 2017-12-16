using System.Drawing;

namespace TagsCloudVisualization
{
    public class TagsCloudVisualizerConfiguration
    {
        public readonly int DefaultPenWidth = 2;

        public Color BackgroundColor { get; }
        public Brush Brush { get; }
        public Pen Pen { get; }
        public Font Font { get; }

        public TagsCloudVisualizerConfiguration(Font font, Colors colors)
        {
            BackgroundColor = colors.BackgroundColor;
            Brush = new SolidBrush(colors.ForegroundColor);
            Pen = new Pen(colors.BackgroundColor, DefaultPenWidth);
            Font = font;
        }
    }

    public class Colors
    {
        public Colors(Color backgroundColor, Color foregroundColor)
        {
            BackgroundColor = backgroundColor;
            ForegroundColor = foregroundColor;
        }

        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
    }
}