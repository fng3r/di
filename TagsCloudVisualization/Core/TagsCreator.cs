using System.Collections.Generic;
using System.Linq;
using TagsCloudVisualization.IO;

namespace TagsCloudVisualization
{
    public class TagsCreator
    {
        private readonly IWordReader wordReader;
        private readonly IStatisticsMaker statisticsMaker;

        public TagsCreator(IWordReader wordReader, IStatisticsMaker statisticsMaker)
        {
            this.wordReader = wordReader;
            this.statisticsMaker = statisticsMaker;
        }

        public CloudTag[] CreateTags(int tagsCount)
        {
            var words = wordReader.ReadWords();
            var mostFrequentWords = statisticsMaker.MakeStatistics(words)
                .OrderByDescending(pair => pair.Value)
                .Take(tagsCount);

            var largestWordCount = mostFrequentWords.First().Value;

            return mostFrequentWords
                .Select(pair => new CloudTag(pair.Key, (double) pair.Value / largestWordCount))
                .ToArray();
        }
    }
}