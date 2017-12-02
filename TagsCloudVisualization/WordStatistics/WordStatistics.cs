using System;
using System.Collections.Generic;
using System.Linq;

namespace TagsCloudVisualization
{

    public static partial class Program
    {
        public class WordStatistics : IWordStatistics
        {
            private static readonly char[] wordDelimiters = { '.', ',', ';', ' ', ':', '(', ')', '[', ']', '\'', '"', '?', '!', '–', '\n' };
            private readonly IWordsLemmatizer lemmatizer;
            private readonly HashSet<PartOfSpeech> allowedPartsOfSpeech = new HashSet<PartOfSpeech>
            {
                PartOfSpeech.Noun,
                PartOfSpeech.Verb,
                PartOfSpeech.Adjective
            };

            public WordStatistics(IWordsLemmatizer lemmatizer) => this.lemmatizer = lemmatizer;

            public Dictionary<string, int> MakeStatistics(IEnumerable<string> lines)
            {

                var words = lines.SelectMany(line => line.Split(wordDelimiters, StringSplitOptions.RemoveEmptyEntries));

                //TODO exclude boring words and words with specific PoS
                return lemmatizer.LemmatizeWords(words)
                    .Where(l => l.Lemma.Length > 3)
                    .Where(l => allowedPartsOfSpeech.Contains(l.PartOfSpeech))
                    .GroupBy(l => l.Lemma)
                    .ToDictionary(group => group.Key, group => group.Count());
            }
        }
    }
}
