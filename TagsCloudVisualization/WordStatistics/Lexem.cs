namespace TagsCloudVisualization
{
    public class Lexem
    {
        public Lexem(string lemma, PartOfSpeech partOfSpeech)
        {
            Lemma = lemma;
            PartOfSpeech = partOfSpeech;
        }

        public string Lemma { get; }
        public PartOfSpeech PartOfSpeech { get; set; }
    }
}