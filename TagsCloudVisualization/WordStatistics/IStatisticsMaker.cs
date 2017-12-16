using System.Collections.Generic;

namespace TagsCloudVisualization
{
    public interface IStatisticsMaker
    {
        Dictionary<string, int> MakeStatistics(IEnumerable<string> words);
    }
}
