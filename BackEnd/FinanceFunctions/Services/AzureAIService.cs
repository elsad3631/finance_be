using Azure;
using Azure.AI.OpenAI;
using FinanceFunctions.IServices;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceFunctions.Services
{
    public class AzureAIService: IAzureAIService
    {
        private readonly AzureOpenAIClient _blobServiceClient;
        private readonly ChatClient _chatClient;
        public AzureAIService(string openAiEndpoint, string openAiKey, string openAiDeployment)
        {
            _blobServiceClient = new AzureOpenAIClient(new Uri(openAiEndpoint), new AzureKeyCredential(openAiKey));
            _chatClient = _blobServiceClient.GetChatClient(openAiDeployment);
        }
        
        public async Task<string> GetResourceData(string cvText)
        {
            try
            {
                string prompt = $@"Sei un assistente AI che analizza curriculum vitae.
                                Estrai e restituisci in JSON le seguenti informazioni:
                                - Name
                                - LastName
                                - DateOfBirth
                                - PlaceOfBirth
                                - Address
                                - Phone
                                - Email
                                - Experiences (Title, Agency, Period)
                                - Skills (Name, Vote (da 1 a 10 in base al livello di esperienza))
                                
                                ATTENZIONE!
                                Le competenze devono essere normalizzate seguendo esclusivamente la seguente lista predefinita. Usa **esattamente questi nomi**. Se trovi sinonimi o varianti, mappale sul nome corretto.

                                Elenco competenze valide:
                                ["".NET"", ""C#"", ""Java"", ""JavaScript"", ""React"", ""Angular"", ""Vue.js"", ""Node.js"", ""Python"", ""SQL Server"", ""Cosmos DB"", ""Azure"", ""Azure Functions"", ""Azure DevOps"", ""Azure Service Bus"", ""Docker"", ""Kubernetes"", ""CI/CD"", ""Git"", ""xUnit"", ""Selenium"", ""Cypress"", ""Microservizi"", ""REST API"", ""NoSQL"", ""Blob Storage"", ""Application Insights"", ""Log Analytics""]

                                Se una competenza non rientra in questa lista, ignorala.

                                Testo del CV: 

                                '''{cvText}'''";

                var chatOptions = new ChatCompletionOptions()
                {
                    Temperature = 0.2f,
                };

                var messages = new List<ChatMessage>() { new UserChatMessage(prompt) };

                var response = await _chatClient.CompleteChatAsync(messages, chatOptions);
                var chatResponse = response.Value.Content.Last().Text;

                int jsonStart = chatResponse.IndexOf('{');
                int jsonEnd = chatResponse.LastIndexOf('}');

                string result = string.Empty;

                if (jsonStart >= 0 && jsonEnd >= 0)
                {
                    result = chatResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetJobData(string title, string description)
        {
            try
            {
                string prompt = $@"Sei un assistente AI specializzato in selezione del personale.
                                Ricevi il titolo e la descrizione di un'offerta di lavoro.
                                Estrai e restituisci un JSON contenente una lista delle competenze tecniche e trasversali richieste per questo ruolo.

                                Formato di output:
                                {{
                                ""Title"": ""<Titolo>"",
                                ""Description"": ""<Descrizione>"",
                                ""Skills"": [""skill1"", ""skill2"", ...]
                                }}

                                Titolo del lavoro: {title}
                                Descrizione: {description}
                                
                                ATTENZIONE!
                                Le competenze devono essere normalizzate seguendo esclusivamente la seguente lista predefinita. Usa **esattamente questi nomi**. Se trovi sinonimi o varianti, mappale sul nome corretto.

                                Elenco competenze valide:
                                ["".NET"", ""C#"", ""Java"", ""JavaScript"", ""React"", ""Angular"", ""Vue.js"", ""Node.js"", ""Python"", ""SQL Server"", ""Cosmos DB"", ""Azure"", ""Azure Functions"", ""Azure DevOps"", ""Azure Service Bus"", ""Docker"", ""Kubernetes"", ""CI/CD"", ""Git"", ""xUnit"", ""Selenium"", ""Cypress"", ""Microservizi"", ""REST API"", ""NoSQL"", ""Blob Storage"", ""Application Insights"", ""Log Analytics""]

                                Se una competenza non rientra in questa lista, ignorala.
                                ";

                var chatOptions = new ChatCompletionOptions()
                {
                    Temperature = 0.2f,
                };

                var messages = new List<ChatMessage>()
                {
                    new SystemChatMessage("Sei un esperto di HR e intelligenza artificiale."),
                    new UserChatMessage(prompt)
                };

                var response = await _chatClient.CompleteChatAsync(messages, chatOptions);
                var chatResponse = response.Value.Content.Last().Text;

                int jsonStart = chatResponse.IndexOf('{');
                int jsonEnd = chatResponse.LastIndexOf('}');

                string result = string.Empty;

                if (jsonStart >= 0 && jsonEnd >= 0)
                {
                    result = chatResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
