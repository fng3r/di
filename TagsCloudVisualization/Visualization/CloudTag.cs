using System.Drawing;

namespace TagsCloudVisualization
{
    public class CloudTag
    {
        public string Text { get; }
        public double Weight { get; }

        public CloudTag(string text, double weight)
        {
            Text = text;
            Weight = weight;
        }
    }
}