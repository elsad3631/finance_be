using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Azure.AI.DocumentIntelligence;
using Azure.Identity;
using Xceed.Words.NET;
using System.Net;
using System.Text.Json;
using FinanceFunctions.Models;
using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;

namespace FinanceFunctions.Functions
{
    public class ResourcesFunctions
    {
        private readonly IResourcesService _resourcesService;
        private readonly string openAiEndpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT");
        private readonly string openAiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");
        private readonly string openAiDeployment = Environment.GetEnvironmentVariable("OPENAI_DEPLOYMENT");
        public ResourcesFunctions(IResourcesService resourcesService)
        {
            _resourcesService = resourcesService;
        }

        [FunctionName("Insert")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Resource/Insert")] HttpRequest req,
            ILogger log)
        {
            try
            {
                // 1. Leggi l'URL del file dalla query string o dal body
                string fileUrl = req.Query["url"];
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Parametro 'url' mancante.")
                    };
                }

                // 2. Scarica il file
                using var httpClient = new HttpClient();
                var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                var memoryStream = new MemoryStream(fileBytes);

                memoryStream.Position = 0;

                string extractedText = string.Empty;
                string extension = Path.GetExtension(fileUrl).ToLower();
                if (extension.Contains("word") || extension.Contains("docx"))
                {
                    using var doc = DocX.Load(memoryStream);
                    extractedText = doc.Text;
                }
                else
                {
                    throw new NotSupportedException("Tipo file non supportato. Solo PDF o DOCX.");
                }

                //Prompt per OpenAI
                string prompt = $@"Sei un assistente AI che analizza curriculum vitae.
                                Estrai e restituisci in JSON le seguenti informazioni:
                                - Name
                                - LastName
                                - Experiences (Title, Agency, Period)
                                - Skills (Name, Vote (da 1 a 10 in base al livello di esperienza))

                                Testo del CV: 

                                '''{extractedText}'''";

                var openAiClient = new AzureOpenAIClient(new Uri(this.openAiEndpoint), new AzureKeyCredential(this.openAiKey));

                var chatClient = openAiClient.GetChatClient(this.openAiDeployment);

                var chatOptions = new ChatCompletionOptions()
                {
                    Temperature = 0.2f,
                };

                var messages = new List<ChatMessage>();
                messages.Add(new UserChatMessage(prompt));

                var response = await chatClient.CompleteChatAsync(messages, chatOptions);
                var chatResponse = response.Value.Content.Last().Text;

                // Opzionale: tenta di isolare solo la parte JSON valida
                int jsonStart = chatResponse.IndexOf('{');
                int jsonEnd = chatResponse.LastIndexOf('}');
                Resource result = new Resource();
                if (jsonStart >= 0 && jsonEnd >= 0)
                {
                    string jsonClean = chatResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    result = JsonSerializer.Deserialize<Resource>(jsonClean);
                    await _resourcesService.CreateAsync(result);
                }

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(result)) };
            }
            catch (Exception ex)
            {
                log.LogError("Errore: " + ex.Message);
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) };
            }
        }
    }
}
