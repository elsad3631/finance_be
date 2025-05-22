using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using FinanceFunctions.Models;

namespace FinanceFunctions.Functions
{
    public class TimerTriggerFunctions
    {
        private readonly INewsService _newsService;
        public TimerTriggerFunctions(INewsService newsService)
        {
            _newsService = newsService;
        }

        //[FunctionName("GetNews")]
        public async Task GetNews([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            #region "Bing"
            //// init with your API key
            //var newsApiClient = new NewsApiClient("4b97e6f0c9e341bfb17e19985002be65");
            //var articlesResponse = newsApiClient.GetEverything(new EverythingRequest
            //{
            //    Q = "Apple",
            //    SortBy = SortBys.Popularity,
            //    Language = Languages.EN,
            //    From = new DateTime(2018, 1, 25)
            //});
            //if (articlesResponse.Status == Statuses.Ok)
            //{
            //    // total results found
            //    log.LogInformation($"Risultati totali: {articlesResponse.TotalResults.ToString()}");
            //    // here's the first 20
            //    foreach (var article in articlesResponse.Articles)
            //    {
            //        // title
            //        log.LogInformation($"Titolo: {article.Title}");
            //        // author
            //        log.LogInformation($"Autore: {article.Author}");
            //        // description
            //        log.LogInformation($"Descrizione: {article.Description}");
            //        // url
            //        log.LogInformation($"URL: {article.Url}");
            //        // published at
            //        log.LogInformation($"Data di pubblicazione: {article.PublishedAt.ToString()}");
            //    }
            //}
            //Console.ReadLine();
            #endregion

            #region "Brave"

            List<string> searches = new List<string>()
            {
                "Stock+Market+News",
                "Economy+News",
                "Today+News",
                "Crypto+Market+News",
                "Bond+Market+News",
                "Government+Bonds+News",
                "Corporate+Bonds+News",
                "World+News",
                "Federal+Reserve+News",
                "European+Central+Bank+News",
                "Inflation+News",
                "Interest+Rates+News",
                "GDP+Growth+News",
                "Energy+Market+News",
                "Oil+Prices+News",
                "Natural+Gas+Prices+News",
                "Geopolitical+News",
                "Political+Crisis+News",
                "Trade+War+News",
                "Sanctions+News",
                "Financial+Regulations+News",
                "Tech+Stocks+News",
                "Banking+Sector+News",
                "Real+Estate+Market+News",
                "Currency+Exchange+News",
                "IMF+World+Bank+News",
                "Government+Spending+News",
                "Fiscal+Policy+News",
                "Monetary+Policy+News",
                "Unemployment+News",
                "Recession+Risk+News",
                "Debt+Ceiling+News",
                "China+Economy+News",
                "US+Politics+News",
                "Germany+Economy+News",
                "European+Union+News",
                "Middle+East+Conflict+News"
            };

            string country = $"ALL";
            string search_lang = "en";
            string freshness = "pd";
            string safesearch = "off";


            using (HttpClient client = new HttpClient())
            {
                // Aggiungi header personalizzati
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("X-Subscription-Token", "BSA59aVUM2fuBdM9RSit7-JdDLtv8Pc");

                try
                {
                    BraveSearchSelectModel apiResponse = new BraveSearchSelectModel();
                    List<NewsSection> news = new List<NewsSection>();

                    foreach (string s in searches)
                    {
                        // L'URL della tua API
                        string url = $"https://api.search.brave.com/res/v1/web/search?q={s}&country={country}&search_lang={search_lang}&safesearch={safesearch}&freshness={freshness}";

                        // Esegui la chiamata GET
                        HttpResponseMessage response = await client.GetAsync(url);

                        // Verifica se la risposta è OK (200)
                        if (response.IsSuccessStatusCode)
                        {
                            // Leggi il contenuto della risposta come stringa
                            string responseBody = await response.Content.ReadAsStringAsync();

                            apiResponse = JsonSerializer.Deserialize<BraveSearchSelectModel>(responseBody);
                            

                            if (apiResponse != null && apiResponse.News != null)
                            {
                                apiResponse.News.Type = s.Replace("+", " ");
                                news.Add(apiResponse.News);

                                log.LogInformation($"News inserite. Search Query: {s}");

                            }
                            else
                            {
                                log.LogWarning($"Non sono state recuperate news. Search Query: {s}");
                            }

                            await Task.Delay(1000);
                        }
                        else
                        {
                            log.LogError($"Errore: {(int)response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }

                    if (apiResponse != null && apiResponse.News != null)
                    {
                        BraveSearch results = new BraveSearch()
                        {
                            News = news.Select(section => new NewsSection
                            {
                                Type = section.Type,
                                Results = section.Results
                                             .DistinctBy(item => item.Title)
                                             .ToList()
                            }).ToList()
                        };

                        await _newsService.CreateAsync(results);

                        log.LogInformation($"News inserite nel Db.");
                    }

                }
                catch (HttpRequestException e)
                {
                    log.LogError($"Eccezione nella richiesta: {e.Message}");
                }
            }

            #endregion
        }
    }
}
