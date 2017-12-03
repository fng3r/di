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
        private readonly string filename;

        public TxtWordReader(string filename) => this.filename = filename;

        public IEnumerable<string> ReadWords()
        {
            return File.ReadLines(filename)
                .SelectMany(line => line.Split(wordDelimiters, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
