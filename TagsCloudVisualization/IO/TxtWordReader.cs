using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagsCloudVisualization.IO
{
    class TxtWordReader : IWordReader
    {
        private static readonly char[] wordDelimiters = { '.', ',', ';', ' ', ':', '(', ')', '[', ']', '\'', '"', '?', '!', '–', '\n' };

        public IEnumerable<string> ReadWords(string filename)
        {
            return File.ReadLines(filename)
                .SelectMany(line => line.Split(wordDelimiters, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
