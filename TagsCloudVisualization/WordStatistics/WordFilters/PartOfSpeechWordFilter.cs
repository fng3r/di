using System.Collections.Generic;

namespace TagsCloudVisualization
{
    public class PartOfSpeechWordFilter : IWordFilter
    {
        private readonly HashSet<PartOfSpeech> allowed;

        public PartOfSpeechWordFilter(IEnumerable<PartOfSpeech> allowedPartsOfSpeech) => allowed = new HashSet<PartOfSpeech>(allowedPartsOfSpeech);

        public bool Filter(Lexem lexem) => allowed.Contains(lexem.PartOfSpeech);
    }
}