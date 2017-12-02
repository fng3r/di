using System.Collections.Generic;

namespace TagsCloudVisualization
{
    public interface IWordsLemmatizer
    {
        IEnumerable<Lexem> LemmatizeWords(IEnumerable<string> words);
    }
}