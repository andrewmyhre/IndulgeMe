using System;

namespace BlessTheWeb.Core.Repository
{
    public class InMemoryDatabaseSession : IDatabaseSession
    {
        private InMemoryDatabase _db;

        public InMemoryDatabaseSession(InMemoryDatabase db)
        {
            this._db = db;
        }

        public void Dispose()
        {
            
        }

        public void SaveChanges()
        {
            
        }

        public void Store<T>(T item)
        {
            _db.Store(item);
        }
    }
}