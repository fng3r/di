using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using Fclp;
using ResultOf;
using TagsCloudVisualization.IO;

namespace TagsCloudVisualization
{
    internal class TagsCloudArgs
    {
        private List<string> boringWords;
        private List<PartOfSpeech> partsOfSpeech;

        public int TagsCount { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int FontSize { get; set; }
        public string FontFamily { get; set; }
        public string ForegroundColor { get; set; }
        public string BackgroundColor { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public List<string> BoringWords
        {
            get => boringWords ?? (boringWords = new List<string>());
            set => boringWords = value;
        }

        public List<PartOfSpeech> PartsOfSpeech
        {
            get => partsOfSpeech ?? (partsOfSpeech = new List<PartOfSpeech>
            {
                PartOfSpeech.Noun,
                PartOfSpeech.Verb,
                PartOfSpeech.Adjective
            });
            set => partsOfSpeech = value;
        }
    }

    public static class Program
    {
        public static void Main(string[] arguments)
        {
            var loadSettingsResult = LoadSettings()
                .RefineError("Failed to load settings from App.config")
                .OnFail(ExitWithMessage);
                
            var parser = CreateArgParser(loadSettingsResult.Value);

            var result = parser.Parse(arguments);
            if (result.HelpCalled) return;
            if (result.HasErrors)
            {
                Console.WriteLine("parameters <src> and <dest> should be specified");
                parser.HelpOption.ShowHelp(parser.Options);
                return;
            }

            var parseArgsResult = parser.Object.AsResult()
                .Then(ValidateSourceFileExists)
                .Then(ValidateFont)
                .RefineError("Invalid arguments")
                .OnFail(ExitWithMessage);

            Run(parseArgsResult.Value, loadSettingsResult.Value);
        }

        private static Result<AppSettings> LoadSettings()
        {
            return Result.Of(() => (AppSettings) System.Configuration.ConfigurationManager.GetSection("settings"));
        }

        private static Result<TagsCloudArgs> ValidateFont(TagsCloudArgs args)
        {
            return args.Validate(a => FontFamily.Families.Select(f => f.Name).Contains(a.FontFamily), 
                $"Font family '{args.FontFamily}' is not found. Try to specify another value or skip this parameter to use defaults");
        }

        private static Result<TagsCloudArgs> ValidateSourceFileExists(TagsCloudArgs args)
        {
            return args.Validate(a => File.Exists(a.Source), $"File '{args.Source}' doesn't exist");
        }

        private static void Run(TagsCloudArgs args, AppSettings settings)
        {
            IContainer container = CreateContainer(args, settings);

            var tagsCreator = container.Resolve<TagsCreator>();
            var layouter = container.Resolve<IRectangleLayouter>();
            var visualizer = container.Resolve<TagsCloudVisualizer>();
            var imageSaver = container.Resolve<IImageSaver>();

            var bitmap = new Bitmap(args.ImageWidth, args.ImageHeight);
            var graphics = Graphics.FromImage(bitmap);

            tagsCreator.CreateTags(args.TagsCount)
                .Then(tags => visualizer.DrawWords(graphics, tags, layouter))
                .Then(_ => imageSaver.SaveImage(args.Destination, bitmap))
                .RefineError("Can't create tags cloud")
                .OnFail(Console.WriteLine)
                .OnSuccess(_ => Console.WriteLine($"Tags cloud successfully saved to '{args.Destination}'"));
        }

        private static FluentCommandLineParser<TagsCloudArgs> CreateArgParser(AppSettings settings)
        {
            var parser = new FluentCommandLineParser<TagsCloudArgs>();

            parser.SetupHelp("h", "help").Callback(help => Console.WriteLine(help));

            parser.Setup(args => args.Source)
                .As("src")
                .WithDescription("source file with text")
                .Required();

            parser.Setup(args => args.Destination)
                .As("dest")
                .WithDescription("file where cloud would be saved")
                .Required();

            parser.Setup(args => args.TagsCount)
                .As('c', "count")
                .WithDescription("number of words which cloud would consists of")
                .SetDefault(settings.TagsCount);

            parser.Setup(args => args.FontSize)
                .As("font-size")
                .WithDescription("max font size of tags in the cloud")
                .SetDefault(settings.Font.Size);

            parser.Setup(args => args.FontFamily)
                .As("font-family")
                .WithDescription("font family for tags in the cloud")
                .SetDefault(settings.Font.Family);

            parser.Setup(args => args.ForegroundColor)
                .As("fg")
                .WithDescription("foreground color")
                .SetDefault(settings.Color.Foreground);

            parser.Setup(args => args.BackgroundColor)
                .As("bg")
                .WithDescription("background color")
                .SetDefault(settings.Color.Background);

            parser.Setup(args => args.ImageWidth)
                .As('w', "width")
                .WithDescription("image width")
                .SetDefault(Screen.PrimaryScreen.Bounds.Width);

            parser.Setup(args => args.ImageHeight)
                .As('h', "height")
                .WithDescription("image height")
                .SetDefault(Screen.PrimaryScreen.Bounds.Height);

            parser.Setup(args => args.BoringWords)
                .As("boring")
                .WithDescription("boring words that should be exluded from cloud");

            parser.Setup(args => args.PartsOfSpeech)
                .As("pos")
                .WithDescription("use only words with sprecified parts of speech in the cloud");

            return parser;
        }

        private static IContainer CreateContainer(TagsCloudArgs args, AppSettings settings)
        {
            var builder = new ContainerBuilder();

            builder.Register(c => new Point(args.ImageWidth / 2, args.ImageHeight / 2));
            builder.RegisterType<Spiral>().As<IPointsGenerator>();
            builder.RegisterType<CircularCloudLayouter>().As<IRectangleLayouter>();

            builder.Register(c => new TxtWordReader(args.Source)).As<IWordReader>();
            builder.Register(c => new MyStemWordLemmatizer(settings.MyStemPath)).As<IWordLemmatizer>();
            builder.Register(c => new PartOfSpeechWordFilter(args.PartsOfSpeech)).As<IWordFilter>();
            builder.Register(c => new BoringWordFilter(args.BoringWords)).As<IWordFilter>();
            builder.RegisterType<StatisticsMaker>().As<IStatisticsMaker>();
            builder.RegisterType<TagsCreator>().AsSelf();

            builder.Register(c => new Font(args.FontFamily, args.FontSize));
            builder.Register(c => new Colors(Color.FromName(args.BackgroundColor), Color.FromName(args.ForegroundColor)));
            builder.RegisterType<TagsCloudVisualizerConfiguration>();
            builder.RegisterType<TagsCloudVisualizer>();

            builder.RegisterType<PngSaver>().As<IImageSaver>();

            var container = builder.Build();
            return container;
        }

        private static void ExitWithMessage(string errorMessage)
        {
            Console.WriteLine(errorMessage);
            Environment.Exit(1);
        }
    }
}
