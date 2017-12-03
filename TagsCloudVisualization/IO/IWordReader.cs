using System.Collections.Generic;

namespace TagsCloudVisualization.IO
{
    public interface IWordReader
    {
        IEnumerable<string> ReadWords();
    }
}
