using System.Drawing;

namespace TagsCloudVisualization
{
    public class CloudTag
    {
        public string Text { get; }
        public Font Font { get; }
        public Size Size { get; }
        public Rectangle Area { get; set; }

        public CloudTag(string text, Font font, Size size)
        {
            Text = text;
            Font = font;
            Size = size;
        }
    }
}