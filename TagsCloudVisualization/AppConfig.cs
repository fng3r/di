using System.Collections.Generic;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    public static class AppConfig
    {
        public static int Width { get; } = Screen.PrimaryScreen.Bounds.Width;
        public static int Height { get; } = Screen.PrimaryScreen.Bounds.Height;
        public static int FontSize { get; } = 80;
        public static string FontFamily { get; } = "Arial";
        public static int TagsCount { get; } = 100;
        public static string ForegroundColor { get; } = "OrangeRed";
        public static string BackgroundColor { get; } = "LightSteelBlue";
        public static string MyStemPath { get; } = "mystem.exe";
        public static List<PartOfSpeech> AllowedPartsOfSpeech = new List<PartOfSpeech>
        {
            PartOfSpeech.Noun,
            PartOfSpeech.Verb,
            PartOfSpeech.Adjective
        };
    }
}