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
        IEnumerable<Indulgence> AllIndulgencesForSin(Guid sinGuid);
        Sin GetSin(object id);
        Indulgence GetIndulgence(string id);
        void GenerateIndulgence(Indulgence indulgence, string fontsDirectory, string contentDirectory);
        byte[] GetIndulgenceImage(Indulgence indulgence, int size);
        byte[] GetIndulgencePdf(Indulgence indulgence);
    }
}