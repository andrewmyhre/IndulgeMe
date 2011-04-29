using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using Raven.Client;
using BlessTheWeb.Core.Extensions;

namespace BlessTheWeb.Core
{
    public interface IIndulgeMeService
    {
        SiteSummaryInfo GetSiteSummaryInfo();
        IEnumerable<Sin> GetFiveRandomBlessedSins();
        IEnumerable<Sin> GetFiveRandomSins();
        IEnumerable<Indulgence> GetFiveLatestIndulgences();
        int IndulgencesCount();
        IEnumerable<Indulgence> AllIndulgences(int page, int pageSize);
        IEnumerable<Indulgence> AllIndulgencesForSin(string sinId);
        Sin GetSin(string id);
        Indulgence GetIndulgence(string id);
        void GenerateIndulgence(string pdfOutputPath, string imageOutputPath, Indulgence indulgence, string fontsDirectory, string contentDirectory);
    }

    public class IndulgeMeService : IIndulgeMeService
    {
        private readonly IDocumentSession _ravenDbSession;

        public IndulgeMeService(IDocumentSession ravenDbSession)
        {
            _ravenDbSession = ravenDbSession;
        }

        public SiteSummaryInfo GetSiteSummaryInfo()
        {
            SiteSummaryInfo siteInfo;
            siteInfo = new SiteSummaryInfo();
            var indulgences = _ravenDbSession.Query<Indulgence>("BlessedIndulgences").ToList();
            siteInfo.TotalDonated = indulgences.Sum(a => a.AmountDonated);
            siteInfo.TotalDonationCount = indulgences.Count();
            var allAbsolvedSins = _ravenDbSession.Query<Sin>("RedeemedSins");
            siteInfo.TotalAbsolvedSins = allAbsolvedSins.Count();
            return siteInfo;
        }

        public IEnumerable<Sin> GetFiveRandomBlessedSins()
        {
            var redeemedSins = _ravenDbSession.Query<Sin>("AllSins").Where(s => s.TotalDonationCount > 0);
            return redeemedSins.Take(5).ToList();

        }

        public IEnumerable<Sin> GetFiveRandomSins()
        {
            return _ravenDbSession.Query<Sin>("AllSins").Random(5);
        }

        public IEnumerable<Indulgence> GetFiveLatestIndulgences()
        {
            return Queryable.Take<Indulgence>(_ravenDbSession.Query<Indulgence>("BlessedIndulgences"), 5);
        }

        public int IndulgencesCount()
        {
            return _ravenDbSession.Query<Indulgence>("BlessedIndulgences").Count();
        }

        public IEnumerable<Indulgence> AllIndulgences(int page, int pageSize)
        {
            return _ravenDbSession.Query<Indulgence>("BlessedIndulgences").Skip(pageSize*page).Take(pageSize);
        }

        public IEnumerable<Indulgence> AllIndulgencesForSin(string sinId)
        {
            return _ravenDbSession.LuceneQuery<Indulgence>("BlessedIndulgences").WhereEquals("SinId", sinId).ToList();
        }

        public Sin GetSin(string id)
        {
            return _ravenDbSession.Load<Sin>(id);
        }

        public Indulgence GetIndulgence(string id)
        {
            return _ravenDbSession.Load<Indulgence>(id);
        }

        public void GenerateIndulgence(string pdfOutputPath, string imageOutputPath, Indulgence indulgence, string fontsDirectory, string contentDirectory)
        {
            new IndulgenceGeneratoriTextSharp().Generate(indulgence, pdfOutputPath, imageOutputPath, indulgence.CharityName, 
                fontsDirectory, contentDirectory);
        }
    }
}
