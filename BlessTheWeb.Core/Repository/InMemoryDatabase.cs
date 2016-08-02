using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlessTheWeb.Core.Extensions;

namespace BlessTheWeb.Core.Repository
{
    public class InMemoryDatabase : IDatabase
    {
        private static IList<Indulgence> _indulgences = new List<Indulgence>();
        private static IList<Sin> _sins = new List<Sin>();

        public void Dispose()
        {
            
        }

        public IEnumerable<Sin> GetAbsolvedSins()
        {
            return _sins.Where(s => s.TotalDonationCount > 0);
        }

        internal void Store<T>(T item)
        {
            if (typeof(T) == typeof(Indulgence))
            {
                var ind = item as Indulgence;
                var existing = _indulgences.SingleOrDefault(i => i.Id == ind.Id);
                {
                    if (existing != null)
                    {
                        ind.Id = existing.Id;
                        _indulgences.Remove(existing);
                        _indulgences.Add(ind);
                    }
                    else
                    {
                        _indulgences.Add(ind);
                        ind.Id = _indulgences.Count();
                    }

                }
            } else if (typeof(T) == typeof(Sin))
            {
                var sin = item as Sin;
                var existing = _sins.SingleOrDefault(s => s.Guid== sin.Guid);
                if (existing != null)
                {
                    _sins.Remove(existing);
                    sin.Id = existing.Id;
                    _sins.Add(sin);
                }
                else
                {
                    _sins.Add(sin);
                    sin.Id = _sins.Count();
                }
            }
        }

        public IEnumerable<Indulgence> GetBlessedIndulgences()
        {
            return _indulgences.Where(i => i.JustGivingDonationId != 0);
        }

        public int GetBlessedIndulgencesCount()
        {
            return GetBlessedIndulgences().Count();
        }

        public IEnumerable<Indulgence> GetFiveLatestIndulgences()
        {
            return _indulgences.Take(5);
        }

        public IEnumerable<Sin> GetFiveRandomAbsolvedSins()
        {
            var pool = _sins.Where(s => s.TotalDonationCount > 0);
            if (pool.Count() <= 5)
                return pool;
            return _sins.Where(s => s.TotalDonationCount > 0).Random(5);
        }

        public IEnumerable<Sin> GetFiveRandomSins()
        {
            if (_sins.Count() <= 5) return _sins;
            return _sins.Random(5);
        }

        public Indulgence GetIndulgence(string id)
        {
            return _indulgences.SingleOrDefault(i => i.Id.ToString() == id);
        }

        public IEnumerable<Indulgence> GetIndulgencesForSin(Guid sinGuid)
        {
            return _indulgences.Where(i => i.SinGuid == sinGuid);
        }

        public IEnumerable<Indulgence> GetPageOfIndulgences(int page, int pageSize)
        {
            return _indulgences.Skip((page - 1)*pageSize).Take(pageSize);
        }

        public Sin GetSin(object id)
        {
            return _sins.SingleOrDefault(s => s.Id == id);
        }

        public IDatabaseSession OpenSession()
        {
            return new InMemoryDatabaseSession(this);
        }

        public Indulgence GetIndulgenceByGuid(string guid)
        {
            Guid g = Guid.Parse(guid);
            return _indulgences.SingleOrDefault(i => i.Guid == g);
        }

        public void Absolve(string guid, int donationId, string donationRef, decimal amount, string reference)
        {
            using (var session = OpenSession())
            {
                var indulgence = GetIndulgenceByGuid(guid);
                indulgence.JustGivingDonationId = donationId;
                indulgence.DonationReference = donationRef;
                indulgence.AmountDonated = amount;
                indulgence.IsBlessed = true;
                var sin = GetSinByGuid(indulgence.SinGuid);
                sin.TotalDonationCount++;
                sin.TotalDonated += amount;
                session.SaveChanges();
            }
        }

        public Sin GetSinByGuid(string guid)
        {
            return GetSinByGuid(Guid.Parse(guid));
        }
        public Sin GetSinByGuid(Guid guid)
        {
            return _sins.SingleOrDefault(s => s.Guid == guid);
        }
    }
}
