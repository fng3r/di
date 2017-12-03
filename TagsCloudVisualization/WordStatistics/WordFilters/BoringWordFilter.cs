using System;
using System.Collections.Generic;

namespace TagsCloudVisualization
{
    public class BoringWordFilter : IWordFilter
    {
        private readonly HashSet<string> boringWords;

        public BoringWordFilter(IEnumerable<string> words)
        {
            boringWords = new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
        }

        public bool Filter(Lexem lexem)
        {
            return !boringWords.Contains(lexem.Lemma);
        }
    }
}