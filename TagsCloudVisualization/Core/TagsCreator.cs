using System.Collections.Generic;
using System.Linq;
using ResultOf;
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

        public Result<CloudTag[]> CreateTags(int tagsCount)
        {
            var words = wordReader.ReadWords();
            var statisticsResult = statisticsMaker.MakeStatistics(words);
            if (!statisticsResult.IsSuccess)
                return Result.Fail<CloudTag[]>(statisticsResult.Error);

            var mostFrequentWords = statisticsResult.Value
                .OrderByDescending(pair => pair.Value)
                .Take(tagsCount)
                .ToArray();

            var largestWordCount = mostFrequentWords[0].Value;

            return mostFrequentWords
                .Select(pair => new CloudTag(pair.Key, (double) pair.Value / largestWordCount))
                .ToArray().AsResult();
        }
    }
}