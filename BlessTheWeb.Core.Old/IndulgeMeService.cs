using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using BlessTheWeb.Core.Extensions;
using BlessTheWeb.Core.Repository;

namespace BlessTheWeb.Core
{
    public class IndulgeMeService : IIndulgeMeService
    {
        private readonly IDatabase _db;
        private readonly IFileStorage _fileStorage;
        private readonly IIndulgenceGenerator _indulgenceGenerator;

        public IndulgeMeService(IDatabase db, IFileStorage fileStorage, IIndulgenceGenerator indulgenceGenerator)
        {
            _db = db;
            _fileStorage = fileStorage;
            _indulgenceGenerator = indulgenceGenerator;
        }

        public SiteSummaryInfo GetSiteSummaryInfo()
        {
            SiteSummaryInfo siteInfo;
            siteInfo = new SiteSummaryInfo();
            var indulgences = _db.GetBlessedIndulgences();
            siteInfo.TotalDonated = indulgences.Sum(a => a.AmountDonated);
            siteInfo.TotalDonationCount = indulgences.Count();
            var allAbsolvedSins =_db.GetAbsolvedSins();
            siteInfo.TotalAbsolvedSins = allAbsolvedSins.Count();
            return siteInfo;
        }

        public IEnumerable<Sin> GetFiveRandomBlessedSins()
        {
            return _db.GetFiveRandomAbsolvedSins().ToList();

        }

        public IEnumerable<Sin> GetFiveRandomSins()
        {
            return _db.GetFiveRandomSins();
        }

        public IEnumerable<Indulgence> GetFiveLatestIndulgences()
        {
            return _db.GetFiveLatestIndulgences();
        }

        public int IndulgencesCount()
        {
            return _db.GetBlessedIndulgencesCount();
        }

        public IEnumerable<Indulgence> AllIndulgences(int page, int pageSize)
        {
            return _db.GetPageOfIndulgences(page, pageSize);
        }

        public IEnumerable<Indulgence> AllIndulgencesForSin(Guid sinGuid)
        {
            return _db.GetIndulgencesForSin(sinGuid);
        }

        public Sin GetSin(object id)
        {
            return _db.GetSin(id);
        }

        public Indulgence GetIndulgence(string id)
        {
            return _db.GetIndulgence(id);
        }

        public void GenerateIndulgence(Indulgence indulgence, string fontsDirectory, string contentDirectory)
        {
            string pdfFilename = string.Format("{0}{1}.pdf", ConfigurationManager.AppSettings["IndulgencePdfRelativePath"], indulgence.Guid);
            string imageThumbnailFileName_1 = string.Format("{0}{1}_1.png", ConfigurationManager.AppSettings["IndulgenceThumbnailRelativePath"], indulgence.Guid);
            string imageThumbnailFileName_2 = string.Format("{0}{1}_2.png", ConfigurationManager.AppSettings["IndulgenceThumbnailRelativePath"], indulgence.Guid);
            string imageThumbnailFileName_3 = string.Format("{0}{1}_3.png", ConfigurationManager.AppSettings["IndulgenceThumbnailRelativePath"], indulgence.Guid);
            string imageThumbnailFileName_4 = string.Format("{0}{1}_4.png", ConfigurationManager.AppSettings["IndulgenceThumbnailRelativePath"], indulgence.Guid);
            _indulgenceGenerator.Generate(indulgence, fontsDirectory, contentDirectory, indulgence.BackgroundImageName,
                pdfFilename, imageThumbnailFileName_1, imageThumbnailFileName_2, imageThumbnailFileName_3, imageThumbnailFileName_4);
        }

        public byte[] GetIndulgenceImage(Indulgence indulgence, int size)
        {
            string imageFileName = string.Format("{0}{1}_{2}.png", ConfigurationManager.AppSettings["IndulgenceThumbnailRelativePath"], indulgence.Guid, size);
            return _fileStorage.Get(imageFileName);
        }

        public byte[] GetIndulgencePdf(Indulgence indulgence)
        {
            string pdfFilename = string.Format("{0}{1}.pdf", ConfigurationManager.AppSettings["IndulgencePdfRelativePath"], indulgence.Guid);
            return _fileStorage.Get(pdfFilename);
        }
    }
}
