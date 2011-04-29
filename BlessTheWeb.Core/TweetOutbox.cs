using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BlessTheWeb.Core
{
    public interface ITweetOutbox
    {
        void Add(Indulgence indulgence);
        IEnumerable<Indulgence> RetrieveAll();
        void ProcessAll(Action<Indulgence> action);
    }
    public class TweetOutbox : ITweetOutbox
    {
        private string _outboxDirectory;
        public TweetOutbox(string outboxDirectory)
        {
            _outboxDirectory = outboxDirectory;
        }
        public void Add(Indulgence indulgence)
        {
            if (!Directory.Exists(_outboxDirectory))
                Directory.CreateDirectory(_outboxDirectory);

            string filename = Path.Combine(_outboxDirectory, string.Format("{0}.xml", Guid.NewGuid()));
            using (var outStream = File.Create(filename))
            {
                XmlWriter xw = XmlWriter.Create(outStream);
                XmlSerializer serializer = new XmlSerializer(typeof(Indulgence));
                serializer.Serialize(xw, indulgence);
                xw.Flush();
                xw.Close();
            }
        }

        public void ProcessAll(Action<Indulgence> action)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Indulgence));
            DirectoryInfo dir = new DirectoryInfo(_outboxDirectory);
            var files = dir.GetFiles("*.xml");
            List<Indulgence> indulgences = new List<Indulgence>();
            foreach (var file in files)
            {
                try
                {
                    using (var inStream = File.OpenRead(file.FullName))
                    {
                        var indulgence = serializer.Deserialize(inStream) as Indulgence;
                        action.Invoke(indulgence);
                        inStream.Close();
                    }
                    file.Delete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Couldn't read indulgence from file {0}", file.Name);
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }

        public IEnumerable<Indulgence> RetrieveAll()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Indulgence));
            DirectoryInfo dir = new DirectoryInfo(_outboxDirectory);
            var files = dir.GetFiles("*.xml");
            List<Indulgence> indulgences = new List<Indulgence>();
            foreach(var file in files)
            {
                try
                {
                    using (var inStream = File.OpenRead(file.FullName))
                    {
                        indulgences.Add(serializer.Deserialize(inStream) as Indulgence);
                        inStream.Close();
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine("Couldn't read indulgence from file {0}", file.Name);
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }

            return indulgences;
        }

        public void Clear()
        {
            DirectoryInfo dir = new DirectoryInfo(_outboxDirectory);
        }
    }
}
