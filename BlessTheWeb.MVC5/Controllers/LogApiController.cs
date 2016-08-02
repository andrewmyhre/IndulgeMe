using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BlessTheWeb.MVC5.Controllers
{
    public class LogController : ApiController
    {
        [HttpGet]
        public object Log()
        {
            var logFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/app_data/log.txt");
            if (System.IO.File.Exists(logFilePath))
            {
                using (var file = System.IO.File.Open(logFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.Read))
                    using (System.IO.StreamReader reader = new StreamReader(file))
                    {
                        return new HttpResponseMessage()
                        {
                            Content = new StringContent(reader.ReadToEnd(), Encoding.UTF8, "text/plain")
                        };
                    }
            }
            return NotFound();
        }
    }
}
