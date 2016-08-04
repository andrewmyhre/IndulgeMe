using FluentNHibernate.Cfg;
using log4net;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.IO;
using System;

namespace BlessTheWeb.Data.NHibernate
{
    public class SessionFactory
    {
        private static ILog log = LogManager.GetLogger(typeof(SessionFactory));
        private static ISessionFactory _sessionFactory = null;
        public static ISessionFactory Instance
        {
            get
            {
                if (_sessionFactory == null)
                    _sessionFactory = CreateSessionFactory();
                return _sessionFactory;
            }
        }
        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently
                .Configure()
                .Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012
                .ConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings["blesstheweb-sql-dev"].ConnectionString))
                .Mappings(x=>x.FluentMappings.AddFromAssemblyOf<IndulgenceMap>())
                .ExposeConfiguration(cfg=>new SchemaUpdate(cfg).Execute(false,true))
                .BuildSessionFactory();
        }

        public static void RebuildDatabase()
        {
            _sessionFactory.Dispose();
            _sessionFactory = Fluently
                .Configure()
                .Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012
                .ConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings["blesstheweb-sql-dev"].ConnectionString))
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<IndulgenceMap>())
                .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                .BuildSessionFactory();
        }
    }
}