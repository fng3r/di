using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace TagsCloudVisualization
{

    public static partial class Program
    {
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
