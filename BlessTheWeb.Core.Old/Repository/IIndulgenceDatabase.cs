using System;
using System.Collections.Generic;

namespace BlessTheWeb.Core.Repository
{
    public interface IDatabase : IDisposable
    {
        IEnumerable<Indulgence> GetBlessedIndulgences();
        IEnumerable<Sin> GetAbsolvedSins();
        IEnumerable<Sin> GetFiveRandomAbsolvedSins();
        IEnumerable<Sin> GetFiveRandomSins();
        IEnumerable<Indulgence> GetFiveLatestIndulgences();
        int GetBlessedIndulgencesCount();
        IEnumerable<Indulgence> GetPageOfIndulgences(int page, int pageSize);
        Sin GetSin(object id);
        IEnumerable<Indulgence> GetIndulgencesForSin(Guid sinGuid);
        Indulgence GetIndulgence(string id);


        IDatabaseSession OpenSession();
        Indulgence GetIndulgenceByGuid(string guid);
        void Absolve(string guid, int donationId, string donationRef, decimal amount, string reference);
        Sin GetSinByGuid(string sinGuid);
        Sin GetSinByGuid(Guid sinGuid);
    }

    public interface IDatabaseSession : IDisposable
    {
        void SaveChanges();
        void Store<T>(T item);
    }
}