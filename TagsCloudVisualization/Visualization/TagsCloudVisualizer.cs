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

        public Bitmap DrawWords(int width, int height, IEnumerable<Word> words)
        {
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(configuration.BackgroundColor);
            foreach (var word in words)
                graphics.DrawString(word.Text, word.Font, configuration.Brush, word.Area);

            return bitmap;
        }
    }

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
