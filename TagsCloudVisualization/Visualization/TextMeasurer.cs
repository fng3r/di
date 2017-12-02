using System;
using System.Drawing;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    public class TextMeasurer
    {
        private readonly string fontFamily;
        private readonly int maxFontSize;

        public TextMeasurer(string fontFamily, int fontSize)
        {
            this.fontFamily = fontFamily;
            maxFontSize = fontSize;
        }

        public CloudTag MeasureText(string text, double weight)
        {
            var wordFontSize = Math.Max(10, (int) (maxFontSize * weight));
            var font = new Font(fontFamily, wordFontSize);
            var size = TextRenderer.MeasureText(text, font);
            return new CloudTag(text, font, size);
        }
    }

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