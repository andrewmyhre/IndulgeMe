namespace BlessTheWeb.Core.Trawlers
{
    public interface IWebPageDownloader
    {
        string GetPage(string url);
    }
}