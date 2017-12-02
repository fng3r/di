using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using Fclp;
using TagsCloudVisualization;
using TagsCloudVisualization.IO;

namespace TagsCloudVisualization
{
    internal class TagsCloudArgs
    {
        public int WordsCount { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int FontSize { get; set; }
        public string FontFamily { get; set; }
        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }
    }

    public static partial class Program
    {
        private static readonly int width = Screen.PrimaryScreen.Bounds.Width;
        private static readonly int height = Screen.PrimaryScreen.Bounds.Height;

        public static void Main(string[] arg)
        {
            var parser = new FluentCommandLineParser<TagsCloudArgs>();

            parser.SetupHelp("h", "help").Callback(help => Console.WriteLine(help));

            parser.Setup(args => args.Source)
                .As("src")
                .WithDescription("source file with text")
                //.Required()
                .SetDefault(@"data\war_and_peace.txt");

            parser.Setup(args => args.Destination)
                .As("dest")
                .WithDescription("file where cloud would be saved")
                //.Required()
                .SetDefault("cloud.jpg");

            parser.Setup(args => args.WordsCount)
                .As('c', "count")
                .WithDescription("number of words which cloud would consists of")
                .SetDefault(100);

            parser.Setup(args => args.FontSize)
                .As("font-size")
                .WithDescription("max font size of tags in the cloud")
                .SetDefault(80);

            parser.Setup(args => args.FontFamily)
                .As("font-family")
                .WithDescription("font family for tags in the cloud")
                .SetDefault("Arial");

            parser.Setup(args => args.ForegroundColor)
                .As("fg")
                .WithDescription("foreground color")
                .SetDefault("OrangeRed");

            parser.Setup(args => args.BackgroundColor)
                .As("bg")
                .WithDescription("background color")
                .SetDefault("LightSteelBlue");

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

            builder.RegisterType<TxtWordReader>().WithParameter("filename", args.Source).As<IWordReader>();
            builder.Register(c => new TextMeasurer(args.FontFamily, args.FontSize));
            builder.Register(c => new TagsCloudVisualizer(
                config => config
                    .SetBackground(Color.FromName(args.BackgroundColor))
                    .SetForeground(Color.FromName(args.ForegroundColor))
            ));

            builder.RegisterType<MyStemWordsLemmatizer>().WithParameter("mystemPath", "mystem.exe").As<IWordsLemmatizer>();
            builder.RegisterType<WordStatistics>().As<IWordStatistics>();

            var container = builder.Build();

            DrawWords(args, container);
        }

        private static void DrawWords(TagsCloudArgs args, IContainer container)
        {
            var lines = File.ReadLines(args.Source);
            var wordReader = container.Resolve<IWordReader>();
            var statistics = container.Resolve<IWordStatistics>();
            var measurer = container.Resolve<TextMeasurer>();
            var words = GetTags(wordReader, statistics, measurer).Take(args.WordsCount);

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

        public static IEnumerable<CloudTag> GetTags(IWordReader wordReader, IWordStatistics statistics, TextMeasurer measurer)
        {
            var words = wordReader.ReadWords();
            var mostFrequentWords = statistics.MakeStatistics(words);
            var largestCount = mostFrequentWords.First().Value;

            var tags = mostFrequentWords
                .Select(pair => (word: pair.Key, weight: (double)pair.Value / largestCount))
                .Select(e => measurer.MeasureText(e.word, e.weight));

            return tags;
        }
    }
}
