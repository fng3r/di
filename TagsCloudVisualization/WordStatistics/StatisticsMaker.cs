using System.Collections.Generic;
using System.Linq;

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

        public Dictionary<string, int> MakeStatistics(IEnumerable<string> words)
        {
            return lemmatizer.LemmatizeWords(words)
                .Where(l => filters.All(filter => filter.Filter(l)))
                .GroupBy(l => l.Lemma)
                .ToDictionary(group => group.Key, group => group.Count());
        }
    }
}
