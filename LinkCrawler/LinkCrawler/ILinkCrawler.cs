using LinkCrawler.Models;
using System.Threading.Tasks;

namespace LinkCrawler
{
    public interface ILinkCrawler
    {
        void CrawlForLinksInResponse(IResponseModel responseModel);
        void ProcessResponse(IResponseModel responseModel);
        Task SendRequestAsync(string crawlUrl, string referrerUrl = "");
        void Start();
        void WriteOutput(IResponseModel responseModel);
    }
}