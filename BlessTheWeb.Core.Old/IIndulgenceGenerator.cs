using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlessTheWeb.Core
{
    public interface IIndulgenceGenerator
    {
        void Generate(Indulgence indulgence, string fontsDirectory, string contentDirectory, string bkFilename, 
            string pdfFilename, string imageThumbnailFileName_1, string imageThumbnailFileName_2, string imageThumbnailFileName_3, string imageThumbnailFileName_4);
    }
}
