﻿using LinkCrawler.Utils.Parsers;
using LinkCrawler.Utils.Settings;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;

namespace LinkCrawler.Tests.UtilsTests.HelpersTests
{
    [TestFixture]
    public class ValidUrlParserTests
    {
        public ValidUrlParser ValidUrlParser { get; set; }
        [SetUp]
        public void SetUp()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            ValidUrlParser = new ValidUrlParser(new Settings(configuration));
        }

        [Test]
        public void Parse_CompleteValidUrl_True()
        {
            var url = "http://www.github.com";
            string parsed;
            var result = ValidUrlParser.Parse(url, out parsed);
            Assert.That(result, Is.True);
            Assert.That(parsed, Is.EqualTo(url));
        }

        [Test]
        public void Parse_UrlNoScheme_True()
        {
            var url = "//www.github.com";
            string parsed;
            var result = ValidUrlParser.Parse(url, out parsed);
            Assert.That(result, Is.True);
            var validUrl = "http:" + url;
            Assert.That(parsed, Is.EqualTo(validUrl));
        }

        [Test]
        public void Parse_UrlOnlyRelativePath_True()
        {
            var relativeUrl = "/relative/path";
            string parsed;
            var result = ValidUrlParser.Parse(relativeUrl, out parsed);
            Assert.That(result, Is.True);
            var validUrl = string.Format("{0}{1}",ValidUrlParser.BaseUrl, relativeUrl);

            Assert.That(parsed, Is.EqualTo(validUrl));
        }
    }
}
