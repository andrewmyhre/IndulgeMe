using BlessTheWeb.Core;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Utils;
using NHibernate.Criterion;
using BlessTheWeb.Core.Repository;
using System.Configuration;

namespace BlessTheWeb.Data.NHibernate
{
    public class NHibernateIndulgeMeService : IIndulgeMeService
    {
        private readonly ISession _session;
        private readonly IIndulgenceGenerator _indulgenceGenerator;
        private readonly IFileStorage _fileStorage;

        public NHibernateIndulgeMeService(ISession session, IIndulgenceGenerator indulgenceGenerator, IFileStorage fileStorage)
        {
            _session = session;
            _indulgenceGenerator = indulgenceGenerator;
            _fileStorage = fileStorage;
        }

        public IEnumerable<Indulgence> AllIndulgences(int page, int pageSize)
        {
            return _session.CreateCriteria<Indulgence>()
                .SetFirstResult((page - 1)*pageSize)
                .SetMaxResults(pageSize)
                .AddOrder(Order.Desc("DateConfessed"))
                .List<Indulgence>();
        }

        public IEnumerable<Indulgence> AllIndulgencesForSin(Sin sin)
        {
            return _session.CreateCriteria<Indulgence>()
                .SetMaxResults(1000)
                .Add(Restrictions.Eq("Sin.Id", sin.Id))
                .AddOrder(Order.Desc("DateConfessed"))
                .List<Indulgence>();
        }

        public void GenerateIndulgence(Indulgence indulgence, string fontsDirectory, string contentDirectory)
        {
            _indulgenceGenerator.Generate(indulgence,
                fontsDirectory,
                contentDirectory,
                indulgence.BackgroundImageName,
                string.Format("{0}{1}.pdf", ConfigurationManager.AppSettings["IndulgencePdfRelativePath"],indulgence.Guid),
                string.Format("{0}{1}_1.png", ConfigurationManager.AppSettings["IndulgenceThumbnailRelativePath"],indulgence.Guid),
                string.Format("{0}{1}_2.png", ConfigurationManager.AppSettings["IndulgenceThumbnailRelativePath"], indulgence.Guid),
                string.Format("{0}{1}_3.png", ConfigurationManager.AppSettings["IndulgenceThumbnailRelativePath"], indulgence.Guid),
                string.Format("{0}{1}_4.png", ConfigurationManager.AppSettings["IndulgenceThumbnailRelativePath"], indulgence.Guid));
        }

        public IEnumerable<Sin> GetSinsByDonationAmount(int page, int pageSize)
        {
            return _session.CreateCriteria<Sin>()
                .SetFirstResult((page - 1) * pageSize)
                .SetMaxResults(pageSize)
                .AddOrder(Order.Desc("TotalDonated"))
                .List<Sin>();
        }

        public IEnumerable<Sin> GetSins(int page, int pageSize)
        {
            return _session.CreateCriteria<Sin>()
                .SetFirstResult((page - 1)*pageSize)
                .SetMaxResults(pageSize)
                .AddOrder(Order.Desc("Id"))
                .List<Sin>();
        }

        public IEnumerable<Indulgence> GetFiveLatestIndulgences()
        {
            return _session.CreateCriteria<Indulgence>()
                .SetMaxResults(5)
                .AddOrder(Order.Desc("DateConfessed"))
                .List<Indulgence>();
        }

        public IEnumerable<Sin> GetFiveRandomBlessedSins()
        {
            return _session.CreateCriteria<Sin>()
                .SetMaxResults(5)
                .AddOrder(Order.Desc("Id"))
                .Add(Restrictions.Gt("TotalDonationCount", 0))
                .List<Sin>();
        }

        public IEnumerable<Sin> GetFiveRandomSins()
        {
            return _session.CreateCriteria<Sin>()
                .SetMaxResults(5)
                .AddOrder(Order.Desc("Id"))
                .List<Sin>();
        }

        public Indulgence GetIndulgence(int id)
        {
            return _session.Get<Indulgence>(id);
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
            using (var tx = _session.BeginTransaction())
            {
                foreach (var sin in sins)
                {
                    _session.Save(sin);
                }
                tx.Commit();
            }
        }

        public void SetCharityDetails(string guid, int charityId, string charityName, string name, string email, string style)
        {
            var indulgence = _session.CreateCriteria<Indulgence>()
                .Add(Restrictions.Eq("Guid", Guid.Parse(guid)))
                .UniqueResult<Indulgence>();
            indulgence.CharityName = charityName.Substring(0, charityName.Length <= 80 ? charityName.Length : 80);
            indulgence.CharityId = charityId;
            indulgence.Name = name.Substring(0, name.Length <= 60 ? name.Length : 60);
            indulgence.DonorEmailAddress = email;
            indulgence.BackgroundImageName = System.IO.Path.GetFileNameWithoutExtension(style);
            _session.SaveOrUpdate(indulgence);
            _session.Flush();
        }

        public Indulgence CreateIndulgenceForSin(string guid)
        {
            var sin = _session.CreateCriteria<Sin>()
                .Add(Restrictions.Eq("Guid", Guid.Parse(guid)))
                .UniqueResult<Sin>();

            var ind = new Indulgence()
            {
                Confession = sin.Content.Substring(0, sin.Content.Length <= 250 ? sin.Content.Length : 250),
                DateConfessed = DateTime.Now,
                IsBlessed = false,
                IsConfession = false,
                Sin = sin
            };
            _session.Save(ind);
            _session.Flush();
            return ind;
        }

        public void Absolve(string guid, int donationId, string donationRef, decimal amount, string reference)
        {
            var indulgence = _session.CreateCriteria<Indulgence>()
                .Add(Restrictions.Eq("Guid", Guid.Parse(guid)))
                .UniqueResult<Indulgence>();
            indulgence.AmountDonated = amount;
            indulgence.DonationReference = donationRef;
            indulgence.JustGivingDonationId = donationId;
            indulgence.Sin.TotalDonated += amount;
            indulgence.Sin.TotalDonationCount++;
            _session.Flush();
        }

        public Sin GetSin(int id)
        {
            return _session.Get<Sin>(id);
        }

        public SiteSummaryInfo GetSiteSummaryInfo()
        {
            SiteSummaryInfo siteInfo;
            siteInfo = new SiteSummaryInfo();
            siteInfo.TotalDonated = _session.CreateCriteria<Indulgence>().SetProjection(Projections.Sum("AmountDonated")).UniqueResult<decimal>();
            siteInfo.TotalDonationCount = _session.CreateCriteria<Indulgence>().Add(Restrictions.Eq("IsBlessed",true)).SetProjection(Projections.Count("AmountDonated")).UniqueResult<int>();
            siteInfo.TotalAbsolvedSins = _session.CreateCriteria<Sin>().SetProjection(Projections.Count("Id")).UniqueResult<int>();
            return siteInfo;
        }

        public int IndulgencesCount()
        {
            return _session.CreateCriteria<Indulgence>()
                .SetProjection(Projections.Count("Id"))
                .UniqueResult<int>();
        }

        public void SaveIndulgence(Indulgence indulgence)
        {
            _session.Save(indulgence);
            _session.Save(indulgence.Sin);
            _session.Flush();
        }

        public Indulgence GetIndulgenceByGuid(string guid)
        {
            return _session.CreateCriteria<Indulgence>()
                .Add(Restrictions.Eq("Guid", Guid.Parse(guid)))
                .UniqueResult<Indulgence>();
        }
    }
}
