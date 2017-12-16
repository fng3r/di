using System.Collections.Generic;
using ResultOf;

namespace TagsCloudVisualization
{
    public interface IStatisticsMaker
    {
        Result<Dictionary<string, int>> MakeStatistics(IEnumerable<string> words);
    }
}
