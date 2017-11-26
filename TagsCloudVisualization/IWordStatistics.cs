using System.Collections.Generic;

namespace TagsCloudVisualization
{

    public static partial class Program
    {
        public interface IWordStatistics
        {
            Dictionary<string, int> MakeStatistics(IEnumerable<string> lines);
        }
    }
}
