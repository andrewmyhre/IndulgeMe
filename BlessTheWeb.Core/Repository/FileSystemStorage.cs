using System;
using System.IO;

namespace BlessTheWeb.Core.Repository
{
    public class FileSystemStorage : IFileStorage
    {
        private static string _baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

        public void Delete(string filePath)
        {
            File.Delete(Path.Combine(_baseDirectory, filePath));
        }

        public bool Exists(string filePath)
        {
            return File.Exists(Path.Combine(_baseDirectory, filePath));
        }

        public byte[] Get(string filePath)
        {
            
            using (
                var file = System.IO.File.Open(Path.Combine(_baseDirectory, filePath), FileMode.Open, FileAccess.Read,
                    FileShare.Read))
            {
                var data = new byte[file.Length];
                file.Read(data, 0, data.Length);
                return data;
            }
        }

        public void Store(string filePath, byte[] data, bool overwrite = false)
        {
            filePath = filePath.Replace('/', '\\');
            var absolutePath = Path.Combine(_baseDirectory, filePath);
            if (!Directory.Exists(Path.GetDirectoryName(absolutePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
            }

            using (var file = File.Open(Path.Combine(_baseDirectory, filePath),
                overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Write))
            {
                file.Write(data, 0, data.Length);
                file.Flush();
            }
        }
    }
}