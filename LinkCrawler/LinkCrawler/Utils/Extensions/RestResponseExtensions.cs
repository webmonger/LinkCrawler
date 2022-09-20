using LinkCrawler.Utils.Settings;
using RestSharp;

namespace LinkCrawler.Utils.Extensions
{
    public static class RestResponseExtensions
    {
        public static bool IsHtmlDocument(this RestResponse restResponse)
        {
            if(restResponse.ContentType == null)
            {
                return false;
            }
            return restResponse.ContentType.StartsWith(Constants.Response.ContentTypeTextHtml);
        }
    }
}
