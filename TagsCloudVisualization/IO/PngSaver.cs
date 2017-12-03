using System.Drawing;

namespace TagsCloudVisualization.IO
{
    class PngSaver : IImageSaver
    {
        public void SaveImage(string filename, Bitmap bitmap) => bitmap.Save(filename);
    }
}