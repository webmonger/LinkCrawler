using System;
using LinkCrawler.Utils.Parsers;
using LinkCrawler.Utils.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using LinkCrawler.Utils.Outputs;

namespace LinkCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            Startup startup = new Startup();

            startup.ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            Run(args, serviceProvider);
        }

        static void Run(string[] args, ServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetService<ISettings>();

            List<IOutput> outputs = new List<IOutput>();
            outputs.Add(serviceProvider.GetService<IOutput>());

            var linkCrawler = new LinkCrawler(outputs, serviceProvider.GetService<IValidUrlParser>(), settings);
            if (args.Length > 0)
            {
                string parsed;
                var validUrlParser = new ValidUrlParser(settings);
                var result = validUrlParser.Parse(args[0], out parsed);
                if (result)
                    linkCrawler.BaseUrl = parsed;
            }
            linkCrawler.Start();
            Console.Read();
        }
    }
}
