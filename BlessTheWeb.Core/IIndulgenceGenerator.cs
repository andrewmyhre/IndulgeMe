using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlessTheWeb.Core
{
    public interface IIndulgenceGenerator
    {
        void Generate(Indulgence indulgence, 
            string pdfOutputPath, string imageOutputPath, string charityName, string fontsDirectory, string contentDirectory);
    }
}
