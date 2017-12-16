using System.Collections.Generic;
using System.Linq;
using ResultOf;

namespace TagsCloudVisualization
{
    public class StatisticsMaker : IStatisticsMaker
    {
        private readonly IWordLemmatizer lemmatizer;
        private readonly IEnumerable<IWordFilter> filters;

        public StatisticsMaker(IWordLemmatizer lemmatizer, IEnumerable<IWordFilter> filters)
        {
            this.lemmatizer = lemmatizer;
            this.filters = filters;
        }

        public Result<Dictionary<string, int>> MakeStatistics(IEnumerable<string> words)
        {
            return Result.Of(() => 
                lemmatizer.LemmatizeWords(words)
                    .Where(l => filters.All(filter => filter.Filter(l)))
                    .GroupBy(l => l.Lemma)
                    .ToDictionary(group => group.Key, group => group.Count())
                    )
                .RefineError("Failed to collect word statistics");
        }
    }
}
