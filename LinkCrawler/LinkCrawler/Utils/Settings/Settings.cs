using LinkCrawler.Utils.Extensions;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Net;

namespace LinkCrawler.Utils.Settings
{
    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string BaseUrl => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.BaseUrl].Trim('/');

        public string ValidUrlRegex => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.ValidUrlRegex];

        public bool CheckImages => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.CheckImages].ToBool();

        public bool OnlyReportBrokenLinksToOutput => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.OnlyReportBrokenLinksToOutput].ToBool();

        public string SlackWebHookUrl => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.SlackWebHookUrl];

        public string SlackWebHookBotName => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.SlackWebHookBotName];

        public string SlackWebHookBotIconEmoji => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.SlackWebHookBotIconEmoji];

        public string SlackWebHookBotMessageFormat => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.SlackWebHookBotMessageFormat];

        public string CsvFilePath => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.CsvFilePath];

        public bool CsvOverwrite => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.CsvOverwrite].ToBool();

        public string CsvDelimiter => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.CsvDelimiter];

        public bool PrintSummary => configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.PrintSummary].ToBool();

        public bool IsSuccess(HttpStatusCode statusCode)
        {
            var configuredCodes = configuration[Constants.AppSettings.RootConfig + Constants.AppSettings.SuccessHttpStatusCodes] ?? "";
            return statusCode.IsSuccess(configuredCodes);
        }
    }
}
