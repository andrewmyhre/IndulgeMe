using System.Collections.Generic;

namespace BlessTheWeb.Core.Repository
{
    public interface IRavenRepository<T>
    {
        T Load(string id);
        void Save(T document);
        void SaveChanges();
        IList<T> RetrieveAll();
    }
}