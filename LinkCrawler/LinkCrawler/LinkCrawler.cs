﻿using LinkCrawler.Models;
using LinkCrawler.Utils.Extensions;
using LinkCrawler.Utils.Helpers;
using LinkCrawler.Utils.Outputs;
using LinkCrawler.Utils.Parsers;
using LinkCrawler.Utils.Settings;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LinkCrawler
{
    public class LinkCrawler : ILinkCrawler
    {
        public string BaseUrl { get; set; }
        public bool CheckImages { get; set; }
        public RestRequest RestRequest { get; set; }
        public IEnumerable<IOutput> Outputs { get; set; }
        public IValidUrlParser ValidUrlParser { get; set; }
        public bool OnlyReportBrokenLinksToOutput { get; set; }
        public static List<LinkModel> UrlList;
        private ISettings _settings;
        private Stopwatch timer;

        public LinkCrawler(IEnumerable<IOutput> outputs, IValidUrlParser validUrlParser, ISettings settings)
        {
            BaseUrl = settings.BaseUrl;
            Outputs = outputs;
            ValidUrlParser = validUrlParser;
            CheckImages = settings.CheckImages;
            UrlList = new List<LinkModel>();
            RestRequest = new RestRequest().SetHeader("Accept", "*/*");
            OnlyReportBrokenLinksToOutput = settings.OnlyReportBrokenLinksToOutput;
            _settings = settings;
            this.timer = new Stopwatch();
        }

        public void Start()
        {
            this.timer.Start();
            UrlList.Add(new LinkModel(BaseUrl));
            SendRequestAsync(BaseUrl);
        }

        public async Task SendRequestAsync(string crawlUrl, string referrerUrl = "")
        {
            var requestModel = new RequestModel(crawlUrl, referrerUrl, BaseUrl);
            var restClientOptions = new RestClientOptions(new Uri(crawlUrl)) { FollowRedirects = false };
            var restClient = new RestClient(restClientOptions);

            var response = await restClient.ExecuteAsync(RestRequest, new System.Threading.CancellationToken());

            if (response == null)
                return;

            var responseModel = new ResponseModel(response, requestModel, _settings);
            ProcessResponse(responseModel);
        }

        public void ProcessResponse(IResponseModel responseModel)
        {
            WriteOutput(responseModel);

            if (responseModel.ShouldCrawl)
                CrawlForLinksInResponse(responseModel);
        }

        public void CrawlForLinksInResponse(IResponseModel responseModel)
        {
            var linksFoundInMarkup = MarkupHelpers.GetValidUrlListFromMarkup(responseModel.Markup, ValidUrlParser, CheckImages);

            foreach (var url in linksFoundInMarkup)
            {
                lock (UrlList)
                {
                    if (UrlList.Where(l => l.Address == url).Count() > 0)
                        continue;

                    UrlList.Add(new LinkModel(url));
                }
                SendRequestAsync(url, responseModel.RequestedUrl);
            }
        }

        public void WriteOutput(IResponseModel responseModel)
        {
            if (!responseModel.IsSuccess)
            {
                foreach (var output in Outputs)
                {
                    output.WriteError(responseModel);
                }
            }
            else if (!OnlyReportBrokenLinksToOutput)
            {
                foreach (var output in Outputs)
                {
                    output.WriteInfo(responseModel);
                }
            }

            CheckIfFinal(responseModel);
        }

        private void CheckIfFinal(IResponseModel responseModel)
        {
            lock (UrlList)
            {

                // First set the status code for the completed link (this will set "CheckingFinished" to true)
                foreach (LinkModel lm in UrlList.Where(l => l.Address == responseModel.RequestedUrl))
                {
                    lm.StatusCode = responseModel.StatusCodeNumber;
                }

                // Then check to see whether there are any pending links left to check
                if ((UrlList.Count > 1) && (UrlList.Where(l => l.CheckingFinished == false).Count() == 0))
                {
                    FinaliseSession();
                }
            }
        }

        private void FinaliseSession()
        {
            this.timer.Stop();
            if (this._settings.PrintSummary)
            {
                List<string> messages = new List<string>();
                messages.Add(""); // add blank line to differentiate summary from main output

                messages.Add("Processing complete. Checked " + UrlList.Count() + " links in " + this.timer.ElapsedMilliseconds.ToString() + "ms");

                messages.Add("");
                messages.Add(" Status | # Links");
                messages.Add(" -------+--------");

                IEnumerable<IGrouping<int, string>> StatusSummary = UrlList.GroupBy(link => link.StatusCode, link => link.Address);
                foreach (IGrouping<int, string> statusGroup in StatusSummary)
                {
                    messages.Add(String.Format("   {0}  | {1,5}", statusGroup.Key, statusGroup.Count()));
                }

                foreach (var output in Outputs)
                {
                    output.WriteInfo(messages.ToArray());
                }
            }
        }
    }
}