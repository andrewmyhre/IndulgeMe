using BlessTheWeb.Core.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BlessTheWeb.MVC5.Controllers
{
    public class ConfigController : ApiController
    {
        private readonly IFileStorage _storage;

        public ConfigController(IFileStorage storage)
        {
            _storage = storage;
        }

        [HttpGet]
        public string CreateAssets()
        {
            var sb = new StringBuilder();
            string[] assets = new string[] {
                "parchment.jpg",
                "parchment2.jpg",
                "parchment3.jpg",
                "YeOldeParchment4.png",
                "marble1.jpg",
                "paper.jpg",
                "home-parchment22.gif"
            };

            foreach (var asset in assets)
            {
                byte[] fileData = null;
                using (
                    var localFile = System.IO.File.Open(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/" + asset),
                        System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {
                    fileData = new byte[localFile.Length];
                    localFile.Read(fileData, 0, fileData.Length);
                }

                _storage.Store(string.Concat(ConfigurationManager.AppSettings["AssetsRelativePath"], asset), fileData, true);
                sb.AppendLine(string.Format("Stored {0}", asset));
            }
            return sb.ToString();
        }
    }
}
