using System.Linq;
using log4net;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Database.Indexing;

namespace BlessTheWeb.Core
{
    public class RavenIndexManager
    {
        private ILog log = LogManager.GetLogger("RavenIndexManager");
        public void SetUpIndexes(IDocumentSession session)
        {
            if (session.DatabaseCommands.GetIndex("AllIndulgences") == null)
            {
                session.DatabaseCommands.PutIndex("AllIndulgences", new IndexDefinition<Indulgence>
                {
                    Map = indulgences => from i in indulgences
                                        select new
                                                    {
                                                        i.CharityId,
                                                        i.Confession,
                                                        i.DateConfessed,
                                                        i.Id,
                                                        i.JustGivingDonationId,
                                                        i.Name,
                                                        i.SinId,
                                                        i.AmountDonated,
                                                        i.DonationReference,
                                                        i.IsConfession,
                                                        i.IsBlessed,
                                                        i.Tweeted
                                                    }
                });
                log.Info("Created Indulgence index");
            }
            if (session.DatabaseCommands.GetIndex("UntweetedIndulgences") == null)
            {
                session.DatabaseCommands.PutIndex("UntweetedIndulgences", new IndexDefinition<Indulgence>
                {
                    Map = indulgences => from i in indulgences
                                         where !i.Tweeted
                                         select new
                                         {
                                             i.CharityId,
                                             i.Confession,
                                             i.DateConfessed,
                                             i.Id,
                                             i.JustGivingDonationId,
                                             i.Name,
                                             i.SinId,
                                             i.AmountDonated,
                                             i.DonationReference,
                                             i.IsConfession,
                                             i.IsBlessed,
                                             i.Tweeted
                                         }
                });
                log.Info("Created Indulgence index");
            }
            if (session.DatabaseCommands.GetIndex("AllSins") == null)
            {
                session.DatabaseCommands.PutIndex("AllSins", new IndexDefinition<Sin>
                    {
                        Map = sins => from s in sins
                                    select new
                                                {
                                                    s.Content,
                                                    s.Id,
                                                    s.Source,
                                                    s.SourceSinId,
                                                    s.TotalDonationCount,
                                                    s.TotalDonated
                                                }
                    });
                log.Info("Created AllSins index");
            }


            if (session.DatabaseCommands.GetIndex("RedeemedSins") == null)
            {
                session.DatabaseCommands.PutIndex("RedeemedSins", new IndexDefinition<Sin>
                    {
                        Map = sins => from s in sins
                                      where s.TotalDonationCount > 0
                                    select new
                                                {
                                                    s.Id,
                                                    s.Content,
                                                    s.Source,
                                                    s.SourceSinId,
                                                    s.TotalDonated,
                                                    s.TotalDonationCount
                                                }

                    });
                log.Info("Created RedeemedSins index");
            }

            if (session.DatabaseCommands.GetIndex("BlessedIndulgences") == null)
            {
                session.DatabaseCommands.PutIndex("BlessedIndulgences", new IndexDefinition<Indulgence>
                    {
                        Map = indulgences => from i in indulgences
                                             where i.IsBlessed
                                             orderby i.Id descending 
                                        select new
                                                    {
                                                        i.CharityId,
                                                        i.Confession,
                                                        i.DateConfessed,
                                                        i.Id,
                                                        i.JustGivingDonationId,
                                                        i.Name,
                                                        i.SinId,
                                                        i.AmountDonated,
                                                        i.DonationReference,
                                                        i.IsConfession,
                                                        i.IsBlessed,
                                                        i.Tweeted
                                                    }

                    });
                log.Info("Created Blessed index");
            }
        }
    }

    public class DonationAggregate
    {
    }
}
