using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using Fclp;

namespace TagsCloudVisualization
{
    internal class TagsCloudArgs
    {
        public int WordsCount { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
    }

    public static partial class Program
    {
        private static readonly int width = Screen.PrimaryScreen.Bounds.Width;
        private static readonly int height = Screen.PrimaryScreen.Bounds.Height;

        public static void Main(string[] arg)
        {
            var parser = new FluentCommandLineParser<TagsCloudArgs>();

            parser.SetupHelp("h", "help").Callback(help => Console.WriteLine(help)).UseForEmptyArgs();

            parser.Setup(args => args.Source)
                .As("src")
                .WithDescription("source file with text")
                .Required();

            parser.Setup(args => args.Destination)
                .As("dest")
                .WithDescription("file where cloud would be saved")
                .Required();

            parser.Setup(args => args.WordsCount)
                .As('c', "count")
                .WithDescription("number of words which cloud would consists of")
                .SetDefault(100);

            var result = parser.Parse(arg);
            if (result.HelpCalled) return;
            if (result.HasErrors)
            {
                Console.WriteLine(result.ErrorText);
                return;
            }

            Run(parser.Object);
        }

        private static void Run(TagsCloudArgs args)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new Point(width / 2, height / 2));
            builder.RegisterType<Spiral>().As<IPointsGenerator>();
            builder.RegisterType<CircularCloudLayouter>().As<IRectangleLayouter>();
            builder.Register(c => new TagsCloudVisualizer(
                config => config
                    .SetBackground(Color.LightSteelBlue)
                    .SetForeground(Color.OrangeRed)
            ));
            builder.RegisterType<SimpleWordStatistics>().As<IWordStatistics>();
            var container = builder.Build();

            DrawWords(args, container);
        }

        private static void DrawWords(TagsCloudArgs args, IContainer container)
        {
            var words = GetWords(File.ReadLines(args.Source), container).Take(args.WordsCount);

            var layouter = container.Resolve<IRectangleLayouter>();
            var visualizer = container.Resolve<TagsCloudVisualizer>();
            words = words.Select(word =>
            {
                word.Area = layouter.PutNextRectangle(word.Size);
                return word;
            });

            var bitmap = visualizer.DrawWords(width, height, words);
            bitmap.Save(args.Destination);
        }

        public static IEnumerable<Word> GetWords(IEnumerable<string> lines, IContainer container)
        {
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
    }
}
