using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    public class TagsCloudVisualizer
    {
        private readonly TagsCloudVisualizerConfiguration config;

        public TagsCloudVisualizer(TagsCloudVisualizerConfiguration config)
        {
            this.config = config;
        }

        public void DrawRectangles(Graphics graphics, IEnumerable<Rectangle> rectangles)
        {
            graphics.Clear(config.BackgroundColor);
            graphics.DrawRectangles(config.Pen, rectangles.ToArray());
        }

        public void DrawWords(Graphics graphics, IEnumerable<CloudTag> words, IRectangleLayouter layouter)
        {
            graphics.Clear(config.BackgroundColor);
            foreach (var word in words)
            {
                var wordFontSize = Math.Max(10, (int)(config.Font.Size * word.Weight));
                var font = new Font(config.Font.FontFamily, wordFontSize);
                var size = TextRenderer.MeasureText(word.Text, font);
                var area = layouter.PutNextRectangle(size);
                graphics.DrawString(word.Text, font, config.Brush, area);
            }
        }
    }
}
