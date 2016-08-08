using System;
using System.Collections.Generic;

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
        IEnumerable<Indulgence> AllIndulgencesForSin(Sin sin);
        Sin GetSin(int id);
        Indulgence GetIndulgence(int id);
        void GenerateIndulgence(Indulgence indulgence, string fontsDirectory, string contentDirectory);
        IEnumerable<Sin> GetSinsByDonationAmount(int page, int pageSize);
        IEnumerable<Sin> GetSins(int page, int pageSize);
        byte[] GetIndulgenceImage(Indulgence indulgence, int size);
        byte[] GetIndulgencePdf(Indulgence indulgence);
        void SaveSins(IEnumerable<Sin> sins);
        void SaveIndulgence(Indulgence indulgence);
        Indulgence GetIndulgenceByGuid(string guid);
        void SetCharityDetails(string guid, int charityId, string charityName, string name, string email, string style);
        Indulgence CreateIndulgenceForSin(string guid);
        void Absolve(string guid, int donationId, string donationRef, decimal amount, string reference);
        void Tweet(Indulgence indulgence);
    }
}