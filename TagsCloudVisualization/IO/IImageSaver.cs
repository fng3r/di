using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagsCloudVisualization.IO
{
    interface IImageSaver
    {
        void SaveImage(string filename, Bitmap bitmap);
    }
}
