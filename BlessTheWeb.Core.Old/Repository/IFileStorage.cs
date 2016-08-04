using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;

namespace BlessTheWeb.Core.Repository
{
    public interface IFileStorage
    {
        bool Exists(string filePath);
        void Delete(string filePath);
        void Store(string filePath, byte[] data, bool overwrite = false);
        byte[] Get(string filePath);
    }
}
