using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using Autofac.Core;

namespace TagsCloudVisualization
{
    public static class Program
    {
        private static readonly int Width = Screen.PrimaryScreen.Bounds.Width;
        private static readonly int Height = Screen.PrimaryScreen.Bounds.Height;

        public static void Main()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new Point(Width / 2, Height / 2));
            builder.RegisterType<Spiral>().As<IPointsGenerator>();
            builder.RegisterType<CircularCloudLayouter>().As<IRectangleLayouter>();
            builder.Register(c => new TagsCloudVisualizer(
                config => config
                    .SetBackground(Color.LightSteelBlue)
                    .SetForeground(Color.OrangeRed)
            ));
            builder.RegisterType<SimpleWordStatistics>().As<IWordStatistics>();
            var container = builder.Build();

            DrawWords(100, container);
        }

        public static void DrawWords(int count, IContainer container)
        {
            IEnumerable<Word> words = GetWords(container).Take(count);

            var layouter = container.Resolve<IRectangleLayouter>();
            var visualizer = container.Resolve<TagsCloudVisualizer>();
            words = words.Select(word =>
            {
                word.Area = layouter.PutNextRectangle(word.Size);
                return word;
            });

            var bitmap = visualizer.DrawWords(Width, Height, words);
            bitmap.Save(@"..\..\Examples\WarAndPeaceCloud.png");
        }

        public static IEnumerable<Word> GetWords(IContainer container)
        {
            var lines = File.ReadLines(@"data\war_and_peace.txt");
            var statistics = container.Resolve<IWordStatistics>();

            var mostFrequentWords = statistics.MakeStatistics(lines)
                .OrderByDescending(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            var largestCount = mostFrequentWords.First().Value;

            var measurer = new TextMeasurer();
            var words = mostFrequentWords
                .Select(pair => (word: pair.Key, weight: (double)pair.Value / largestCount))
                .Select(e => measurer.MeasureText(e.word, e.weight));

            return words;
        }

        public interface IWordStatistics
        {
            Dictionary<string, int> MakeStatistics(IEnumerable<string> lines);
        }

        public class SimpleWordStatistics : IWordStatistics
        {
            private static readonly char[] wordDelimiters = { '.', ',', ';', ' ', ':', '(', ')', '[', ']', '\'', '"', '?', '!', '–', '\n' };

            public Dictionary<string, int> MakeStatistics(IEnumerable<string> lines)
            {
                return lines
                    .SelectMany(line => line.Split(wordDelimiters, StringSplitOptions.RemoveEmptyEntries))
                    .Where(w => w.Length > 3)
                    .Select(w => w.ToLower())
                    .GroupBy(w => w)
                    .ToDictionary(group => group.Key, group => group.Count());

            }
        }
    }
}
