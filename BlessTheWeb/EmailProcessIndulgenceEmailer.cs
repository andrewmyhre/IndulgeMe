using System.Collections.Generic;
using System.IO;
using System.Web;
using BlessTheWeb.Core;
using EmailProcessing;
using JustConfessing;

namespace BlessTheWeb
{
    public class EmailProcessIndulgenceEmailer : IIndulgenceEmailer
    {
        public void Send(Indulgence indulgence, string indulgenceFilePath)
        {
            MvcApplication.EmailFacade.AddEmailToQueue(
                new string[] { indulgence.DonorEmailAddress }, 
                "IndulgenceCreated",
                new Dictionary<string, string>()
                    {
                        {"DonorName", indulgence.Name},
                        {"CharityName", indulgence.CharityName},
                        {"IndulgenceId", indulgence.Id},
                        {"ServerAuthority", "http://" + HttpContext.Current.Request.Url.Authority}
                    },
                new FileInfo[]{new FileInfo(indulgenceFilePath)});
        }
    }
}