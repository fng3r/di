﻿using System.Collections.Generic;

namespace TagsCloudVisualization
{
    public class PosWordFilter : IWordFilter
    {
        private readonly HashSet<PartOfSpeech> allowed = new HashSet<PartOfSpeech>
        {
            PartOfSpeech.Noun,
            PartOfSpeech.Verb,
            PartOfSpeech.Adjective
        };

        public PosWordFilter(IEnumerable<PartOfSpeech> allowedPartsOfSpeech = null)
        {
            if (allowedPartsOfSpeech != null)
                allowed = new HashSet<PartOfSpeech>(allowedPartsOfSpeech);
        }

        public bool Filter(Lexem lexem)
        {
            return allowed.Contains(lexem.PartOfSpeech);
        }
    }
}