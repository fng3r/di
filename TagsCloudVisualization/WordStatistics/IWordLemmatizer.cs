using System.Collections.Generic;

namespace TagsCloudVisualization
{
    public interface IWordLemmatizer
    {
        IEnumerable<Lexem> LemmatizeWords(IEnumerable<string> words);
    }
}