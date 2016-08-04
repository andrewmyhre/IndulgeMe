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
        private readonly IFileStorage _fileStorage;
        private readonly IIndulgenceGenerator _indulgenceGenerator;
        private static List<object> _db = new List<object>();

        public IndulgeMeService(IFileStorage fileStorage, IIndulgenceGenerator indulgenceGenerator)
        {
            _fileStorage = fileStorage;
            _indulgenceGenerator = indulgenceGenerator;
        }

        public SiteSummaryInfo GetSiteSummaryInfo()
        {
            SiteSummaryInfo siteInfo;
            siteInfo = new SiteSummaryInfo();
            var indulgences = _db.Where(o => o is Indulgence).Select(o=>o as Indulgence).ToList();
            siteInfo.TotalDonated = indulgences.Sum(a => a.AmountDonated);
            siteInfo.TotalDonationCount = indulgences.Count();
            var allAbsolvedSins =_db.Where(o=>o is Sin).Select(o=>o as Sin).ToList();
            siteInfo.TotalAbsolvedSins = allAbsolvedSins.Count();
            return siteInfo;
        }

        public IEnumerable<Sin> GetFiveRandomBlessedSins()
        {
            return _db.Where(o => o is Sin).Select(o => o as Sin).Where(s=>s.TotalDonationCount>0).Take(5).ToList();

        }

        public void Absolve(string guid, int donationId, string donationRef, decimal amount, string reference)
        {
            var indulgence = _db.Where(o => o is Indulgence).Select(o => o as Indulgence)
                .SingleOrDefault(i => i.Guid == Guid.Parse(guid));
            indulgence.JustGivingDonationId = donationId;
            indulgence.DonationReference = donationRef;
            indulgence.AmountDonated = amount;
        }

        public IEnumerable<Sin> GetFiveRandomSins()
        {
            return _db.Where(o => o is Sin).Select(o => o as Sin).Take(5);
        }

        public IEnumerable<Indulgence> GetFiveLatestIndulgences()
        {
            return _db.Where(o => o is Indulgence).Select(o => o as Indulgence).OrderByDescending(i => i.Id).Take(5);
        }

        public int IndulgencesCount()
        {
            return _db.Where(o => o is Indulgence).Select(o => o as Indulgence).Count();
        }

        public IEnumerable<Indulgence> AllIndulgences(int page, int pageSize)
        {
            return _db.Where(o => o is Indulgence).Select(o => o as Indulgence).Skip((page-1)*pageSize).Take(pageSize);
        }

        public IEnumerable<Indulgence> AllIndulgencesForSin(Sin sin)
        {
            return _db.Where(o => o is Indulgence).Select(o => o as Indulgence).Where(i=>i.Sin.Id==sin.Id);
        }

        public Sin GetSin(int id)
        {
            return _db.Where(o => o is Sin).Select(o => o as Sin).SingleOrDefault(s => s.Id == id);
        }

        public Indulgence GetIndulgence(int id)
        {
            return _db.Where(o => o is Indulgence).Select(o => o as Indulgence).SingleOrDefault(i => i.Id == id);
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

        public IEnumerable<Sin> GetSinsByDonationAmount(int page, int pageSize)
        {
            return _db.Where(o => o is Sin).Select(o => o as Sin)
                .OrderByDescending(s=>s.TotalDonated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public IEnumerable<Sin> GetSins(int page, int pageSize)
        {
            return _db.Where(o => o is Sin).Select(o => o as Sin)
                .Skip((page - 1)*pageSize)
                .Take(pageSize);
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

        public void SaveSins(IEnumerable<Sin> sins)
        {
            _db.AddRange(sins);
        }

        public void SetCharityDetails(string guid, int charityId, string charityName, string name, string email)
        {
            var indulgence =
                _db.Where(o => o is Indulgence).Select(o => o as Indulgence).SingleOrDefault(i => i.Guid == Guid.Parse(guid));
            indulgence.CharityName = charityName.Substring(0, charityName.Length <= 80 ? charityName.Length : 80);
            indulgence.CharityId = charityId;
            indulgence.Name = name.Substring(0, name.Length <= 60 ? name.Length : 60);
            indulgence.DonorEmailAddress = email;
        }

        public Indulgence CreateIndulgenceForSin(string guid)
        {
            var sin = _db.Where(o => o is Sin).Select(o => o as Sin).SingleOrDefault(s => s.Guid == Guid.Parse(guid));
            var ind = new Indulgence()
            {
                Confession = sin.Content.Truncate(250),
                DateConfessed = DateTime.Now,
                IsBlessed = false,
                IsConfession = false,
                Sin = sin
            };
            _db.Add(ind);
            return ind;
        }

        public void SaveIndulgence(Indulgence indulgence)
        {
            _db.Add(indulgence);
        }

        public Indulgence GetIndulgenceByGuid(string guid)
        {
            return _db.Where(o => o is Indulgence).Select(o => o as Indulgence).SingleOrDefault(i => i.Guid == Guid.Parse(guid));
        }
    }
}
