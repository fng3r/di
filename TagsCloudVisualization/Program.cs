using System;
using System.Drawing;
using Autofac;
using Fclp;
using TagsCloudVisualization.IO;

namespace TagsCloudVisualization
{
    internal class TagsCloudArgs
    {
        public int TagsCount { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int FontSize { get; set; }
        public string FontFamily { get; set; }
        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
    }

    public static class Program
    {
        public static void Main(string[] arguments)
        {
            var parser = new FluentCommandLineParser<TagsCloudArgs>();

            parser.SetupHelp("h", "help").Callback(help => Console.WriteLine(help));

            parser.Setup(args => args.Source)
                .As("src")
                .WithDescription("source file with text")
                .SetDefault(@"data\war_and_peace.txt");

            parser.Setup(args => args.Destination)
                .As("dest")
                .WithDescription("file where cloud would be saved")
                .SetDefault("cloud.png");

            parser.Setup(args => args.TagsCount)
                .As('c', "count")
                .WithDescription("number of words which cloud would consists of")
                .SetDefault(AppConfig.TagsCount);

            parser.Setup(args => args.FontSize)
                .As("font-size")
                .WithDescription("max font size of tags in the cloud")
                .SetDefault(AppConfig.FontSize);

            parser.Setup(args => args.FontFamily)
                .As("font-family")
                .WithDescription("font family for tags in the cloud")
                .SetDefault(AppConfig.FontFamily);

            parser.Setup(args => args.ForegroundColor)
                .As("fg")
                .WithDescription("foreground color")
                .SetDefault(AppConfig.ForegroundColor);

            parser.Setup(args => args.BackgroundColor)
                .As("bg")
                .WithDescription("background color")
                .SetDefault(AppConfig.BackgroundColor);

            parser.Setup(args => args.ImageWidth)
                .As('w', "width")
                .WithDescription("image width")
                .SetDefault(AppConfig.Width);

            parser.Setup(args => args.ImageHeight)
                .As('h', "height")
                .WithDescription("image height")
                .SetDefault(AppConfig.Height);

            var result = parser.Parse(arguments);
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
            IContainer container = CreateContainer(args);

            var layouter = container.Resolve<IRectangleLayouter>();
            var visualizer = container.Resolve<TagsCloudVisualizer>();
            var tagsCreator = container.Resolve<TagsCreator>();

            var tags = tagsCreator.CreateTags(args.TagsCount);
            var bitmap = new Bitmap(args.ImageWidth, args.ImageHeight);
            var graphics = Graphics.FromImage(bitmap);

            visualizer.DrawWords(graphics, tags, layouter);
            bitmap.Save(args.Destination);
        }

        private static IContainer CreateContainer(TagsCloudArgs args)
        {
            var builder = new ContainerBuilder();

            builder.Register(c => new Point(args.ImageWidth / 2, args.ImageHeight / 2));
            builder.RegisterType<Spiral>().As<IPointsGenerator>();
            builder.RegisterType<CircularCloudLayouter>().As<IRectangleLayouter>();

            builder.RegisterType<TxtWordReader>().WithParameter("filename", args.Source).As<IWordReader>();
            builder.RegisterType<MyStemWordLemmatizer>().WithParameter("mystemPath", AppConfig.MyStemPath).As<IWordLemmatizer>();
            builder.RegisterType<PosWordFilter>().As<IWordFilter>();
            builder.RegisterType<BoringWordFilter>().As<IWordFilter>();
            builder.RegisterType<StatisticsMaker>().As<IStatisticsMaker>();
            builder.RegisterType<TagsCreator>().AsSelf();

            builder.Register(c => new Font(args.FontFamily, args.FontSize));
            builder.RegisterType<TagsCloudVisualizerConfiguration>().OnActivated(
                config => config.Instance
                    .SetBackground(Color.FromName(args.BackgroundColor))
                    .SetForeground(Color.FromName(args.ForegroundColor))
                    .SetFont(config.Context.Resolve<Font>())
            );
            builder.RegisterType<TagsCloudVisualizer>();

            var container = builder.Build();
            return container;
        }
    }
}
