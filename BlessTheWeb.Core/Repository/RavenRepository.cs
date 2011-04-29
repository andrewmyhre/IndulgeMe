using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;

namespace BlessTheWeb.Core.Repository
{
    public class RavenRepository<T>: IRavenRepository<T>, IDisposable
    {
        public bool AutoSaveOnDispose { get; set; }
        private readonly IDocumentSession _documentSesssion;
        
        public RavenRepository(IDocumentSession documentSesssion)
        {
            _documentSesssion = documentSesssion;
        }

        public IDocumentSession Session
        {
            get { return _documentSesssion; }
        }

        public T Load(string id)
        {
            return Session.Load<T>(id);
        }


        public void Save(T document)
        {
            Session.Store(document);
        }

        public void SaveChanges()
        {
            Session.SaveChanges();
        }
        
        public IList<T> RetrieveAll()
        {
            return Session.Query<T>("hello").ToList();
        }

        public void Delete(T document)
        {
            Session.Delete(document);
        }

        public void Dispose()
        {
            if(AutoSaveOnDispose)
            {
                Session.SaveChanges();
            }
        }
    }
}
