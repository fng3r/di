using System;
using System.Drawing;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    public class TextMeasurer
    {
        private int defaultFontSize = 60;
        private string defaultFontFamily = "Arial";

        public Word MeasureText(string text, double weight)
        {
            var wordFontSize = Math.Max(8, (int) (defaultFontSize * weight));
            var font = new Font(defaultFontFamily, wordFontSize);
            var size = TextRenderer.MeasureText(text, font);
            return new Word(text, font, size);
        }
    }

    public class Word
    {
        public string Text { get; }
        public Font Font { get; }
        public Size Size { get; }
        public Rectangle Area { get; set; }

        public Word(string text, Font font, Size size)
        {
            Text = text;
            Font = font;
            Size = size;
        }
    }
}