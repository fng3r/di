using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class TagsCloudVisualizer
    {
        private readonly TagsCloudVisualizerConfiguration configuration;

        public TagsCloudVisualizer(TagsCloudVisualizerConfiguration config)
        {
            configuration = config;
        }

        public void DrawRectangles(Graphics graphics, IEnumerable<Rectangle> rectangles)
        {
            graphics.Clear(configuration.BackgroundColor);
            graphics.DrawRectangles(configuration.Pen, rectangles.ToArray());
        }

        public void DrawWords(Graphics graphics, IEnumerable<CloudTag> words)
        {
            graphics.Clear(configuration.BackgroundColor);
            foreach (var word in words)
                graphics.DrawString(word.Text, word.Font, configuration.Brush, word.Area);
        }
    }
}
