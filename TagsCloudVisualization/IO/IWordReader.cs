using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagsCloudVisualization.IO
{
    public interface IWordReader
    {
        IEnumerable<string> ReadWords(string filename);
    }
}
