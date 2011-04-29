namespace BlessTheWeb.Core.Trawlers
{
    public interface ISinTrawler
    {
        TrawlerResult GetSins();
        string SourceName { get; }
    }
}