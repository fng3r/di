using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class TagsCloudVisualizer
    {
        private TagsCloudVisualizerConfiguration configuration;

        public TagsCloudVisualizer(Func<TagsCloudVisualizerConfiguration, TagsCloudVisualizerConfiguration> config)
        {
            configuration = config(new TagsCloudVisualizerConfiguration());
        }

        public Bitmap DrawRectangles(int width, int height, IEnumerable<Rectangle> rectangles)
        {
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(configuration.BackgroundColor);
            graphics.DrawRectangles(configuration.Pen, rectangles.ToArray());

            return bitmap;
        }

        public Bitmap DrawWords(int width, int height, IEnumerable<CloudTag> words)
        {
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(configuration.BackgroundColor);
            foreach (var word in words)
                graphics.DrawString(word.Text, word.Font, configuration.Brush, word.Area);

            return bitmap;
        }
    }
}
