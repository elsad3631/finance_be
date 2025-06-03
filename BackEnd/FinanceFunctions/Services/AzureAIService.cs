using Azure;
using Azure.AI.OpenAI;
using FinanceFunctions.CosmosEntities;
using FinanceFunctions.IServices;
using FinanceFunctions.Models.EmployeeModels;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinanceFunctions.Services
{
    public class AzureAIService : IAzureAIService
    {
        private readonly AzureOpenAIClient _blobServiceClient;
        private readonly ChatClient _chatClient;
        public AzureAIService(string openAiEndpoint, string openAiKey, string openAiDeployment)
        {
            _blobServiceClient = new AzureOpenAIClient(new Uri(openAiEndpoint), new AzureKeyCredential(openAiKey));
            _chatClient = _blobServiceClient.GetChatClient(openAiDeployment);
        }

        // Lista normalizzata di competenze (puoi caricarla da un file di configurazione o un'altra fonte)
        private readonly List<string> _normalizedSkills = new List<string>
        {
            ".NET", "C#", "Java", "JavaScript", "React", "Angular", "Vue.js", "Node.js", "Python",
            "SQL Server", "Cosmos DB", "Azure", "Azure Functions", "Azure DevOps", "Azure Service Bus",
            "Docker", "Kubernetes", "CI/CD", "Git", "xUnit", "Selenium", "Cypress", "Microservizi",
            "REST API", "NoSQL", "Blob Storage", "Application Insights", "Log Analytics",
            // Soft skills aggiunte per completezza se l'AI le estrarrà
            "Comunicazione", "Lavoro di Squadra", "Problem Solving", "Leadership", "Adattabilità",
            "Pensiero Critico", "Gestione del Tempo", "Negoziazione", "Intelligenza Emotiva"
        };

        /// <summary>
        /// Estrae dati anagrafici, esperienze e competenze da un testo di CV.
        /// </summary>
        /// <param name="cvText">Il testo del curriculum vitae.</param>
        /// <returns>Un oggetto Employee popolato con i dati estratti.</returns>
        public async Task<Employee> GetCVData(string cvText)
        {
            try
            {
                string skillsList = string.Join("\", \"", _normalizedSkills);

                string prompt = $@"Sei un assistente AI esperto nell'analisi di curriculum vitae.
                                    Ti fornirò il testo di un curriculum e dovrai estrarre le seguenti informazioni in formato JSON.

                                    **Informazioni da estrarre:**
                                    - **FirstName**: Nome della persona.
                                    - **LastName**: Cognome della persona.
                                    - **DateOfBirth**: Data di nascita (formato 'AAAA-MM-GG' o 'null' se non trovata).
                                    - **PlaceOfBirth**: Luogo di nascita.
                                    - **Address**: Indirizzo completo.
                                    - **Phone**: Numero di telefono.
                                    - **Email**: Indirizzo email.
                                    - **Experiences**: Un array di oggetti Experience. Ogni oggetto deve avere:
                                        - **JobTitle**: Titolo del ruolo.
                                        - **CompanyName**: Nome dell'azienda.
                                        - **StartDate**: Data di inizio (formato 'AAAA-MM-GG').
                                        - **EndDate**: Data di fine (formato 'AAAA-MM-GG' o 'null' se corrente).
                                        - **Description**: Breve descrizione delle responsabilità/logiche (massimo 100 parole).
                                        - **TechnologiesUsed**: Un array di stringhe, tecnologie utilizzate in questa esperienza, normalizzate dalla lista fornita.
                                    - **HardSkills**: Un array di oggetti EmployeeSkill. Ogni oggetto deve avere:
                                        - **Name**: Nome della competenza, normalizzato dalla lista fornita.
                                        - **ProficiencyLevel**: Livello di competenza (integer da 1 a 10, 1=base, 10=esperto).
                                        - **LastUsed**: Data dell'ultima volta che la skill è stata menzionata/utilizzata (formato 'AAAA-MM-GG' o 'null').
                                        - **Certification**: Se esiste una certificazione specifica per la skill (stringa o null).
                                    - **SoftSkills**: Un array di oggetti EmployeeSkill, per le competenze trasversali.

                                    **Regole di normalizzazione delle competenze:**
                                    Le competenze (sia in Experiences.TechnologiesUsed che in HardSkills e SoftSkills) devono essere normalizzate e incluse **esclusivamente** se presenti nella seguente lista predefinita. Usa **esattamente questi nomi**. Se trovi sinonimi o varianti, mappale sul nome corretto.

                                    Lista di competenze valide (non generare altre competenze che non siano in questa lista):
                                    [""{skillsList}""]

                                    **Output JSON desiderato:**
                                    ```json
                                    {{
                                        ""firstName"": ""Mario"",
                                        ""lastName"": ""Rossi"",
                                        ""dateOfBirth"": ""1990-01-15"",
                                        ""placeOfBirth"": ""Roma"",
                                        ""address"": ""Via Roma 1, 00100 Roma"",
                                        ""phone"": ""+39123456789"",
                                        ""email"": ""mario.rossi@example.com"",
                                        ""experiences"": [
                                            {{
                                                ""jobTitle"": ""Software Developer"",
                                                ""companyName"": ""Tech Solutions"",
                                                ""startDate"": ""2018-03-01"",
                                                ""endDate"": ""2022-06-30"",
                                                ""description"": ""Sviluppo di applicazioni web e API REST con C# e .NET."",
                                                ""technologiesUsed"": ["".NET"", ""C#"", ""REST API""]
                                            }}
                                        ],
                                        ""hardSkills"": [
                                            {{
                                                ""name"": ""C#"",
                                                ""proficiencyLevel"": 8,
                                                ""lastUsed"": ""2024-05-01"",
                                                ""certification"": ""Microsoft Certified""
                                            }}
                                        ],
                                        ""softSkills"": [
                                            {{
                                                ""name"": ""Lavoro di Squadra"",
                                                ""proficiencyLevel"": 9,
                                                ""lastUsed"": null,
                                                ""certification"": null
                                            }}
                                        ]
                                    }}'''{cvText}'''";


                var chatOptions = new ChatCompletionOptions()
                {
                    Temperature = 0.2f, // Mantieni un valore basso per risposte più consistenti
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(), // Richiedi output JSON esplicito
                };

                var messages = new List<ChatMessage>()
                {
                    new SystemChatMessage("Sei un assistente AI molto preciso ed efficiente. Estrai informazioni dai CV e restituiscile in formato JSON strutturato, normalizzando le competenze sulla lista fornita."),
                    new UserChatMessage(prompt)
                };

                var response = await _chatClient.CompleteChatAsync(messages, chatOptions);
                var chatResponse = response.Value.Content.Last().Text;

                // Deserialize direttamente nell'oggetto Employee
                var result = System.Text.Json.JsonSerializer.Deserialize<Employee>(chatResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetCVData: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Estrae le competenze hard e soft richieste da una descrizione di progetto.
        /// </summary>
        /// <param name="title">Il titolo del progetto.</param>
        /// <param name="description">La descrizione del progetto.</param>
        /// <returns>Un oggetto Project popolato con le RequiredHardSkills e RequiredSoftSkills.</returns>
        public async Task<Project> GetProjectSkills(string title, string description)
        {
            try
            {
                string skillsList = string.Join("\", \"", _normalizedSkills);

                string prompt = $@"Sei un assistente AI specializzato nell'analisi di descrizioni di progetti e job roles.
                                    Ti fornirò il titolo e la descrizione di un progetto. Il tuo compito è estrarre e restituire in formato JSON una lista delle competenze tecniche (hard skills) e trasversali (soft skills) richieste per implementare questo progetto.

                                    Informazioni da estrarre:

                                    Name: Il titolo del progetto.
                                    Description: La descrizione del progetto.
                                    RequiredHardSkills: Un array di oggetti SkillRequirement. Ogni oggetto deve avere:
                                    Name: Nome della competenza tecnica, normalizzato dalla lista fornita.
                                    MinProficiencyLevel: Livello minimo di competenza richiesto (integer da 1 a 10).
                                    IsMandatory: Booleano, indica se la competenza è obbligatoria.
                                    RequiredSoftSkills: Un array di stringhe, nomi delle competenze trasversali, normalizzati dalla lista fornita.
                                    Regole di normalizzazione delle competenze:
                                    Le competenze (sia hard che soft) devono essere normalizzate e incluse esclusivamente se presenti nella seguente lista predefinita. Usa esattamente questi nomi. Se trovi sinonimi o varianti, mappale sul nome corretto.

                                    Lista di competenze valide (non generare altre competenze che non siano in questa lista):
                                    [""{skillsList}""]
                                    {{
                                        ""name"": """"Sviluppo Piattaforma E-commerce"""",
                                        ""description"""": ""Progetto per la realizzazione di una nuova piattaforma e-commerce scalabile basata su microservizi."""",
                                        ""requiredHardSkills"": [
                                            {{
                                                ""name"": "".NET"",
                                                ""minProficiencyLevel"": 8,
                                                ""isMandatory"": true
                                            }},
                                            {{
                                                ""name"""": ""Microservizi"",
                                                ""minProficiencyLevel"""": 7,
                                                ""isMandatory"": true
                                            }},
                                            {{
                                                ""name"": ""Azure Functions"",
                                                ""minProficiencyLevel"": 6,
                                                ""isMandatory"": false
                                            }}
                                        ],
                                        ""requiredSoftSkills"": [
                                            ""Lavoro di Squadra"",
                                            ""Problem Solving""
                                        ]
                                    }}
                                    Titolo del progetto: {title}
                                    Descrizione del progetto: {description}";

                var chatOptions = new ChatCompletionOptions()
                {
                    Temperature = 0.2f,
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                };

                var messages = new List<ChatMessage>()
        {
            new SystemChatMessage("Sei un esperto di HR e analisi di requisiti di progetto. Estrai con precisione le skill richieste e normalizzale secondo la lista data, fornendo output JSON."),
            new UserChatMessage(prompt)
        };

                var response = await _chatClient.CompleteChatAsync(messages, chatOptions);
                var chatResponse = response.Value.Content.Last().Text;

                var result = System.Text.Json.JsonSerializer.Deserialize<Project>(chatResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetProjectSkills: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Trova i dipendenti più adatti per un progetto basandosi sulle competenze richieste.
        /// </summary>
        /// <param name="project">Il progetto con le competenze richieste.</param>
        /// <param name="availableEmployees">La lista di tutti i dipendenti disponibili.</param>
        /// <returns>Una lista di EmployeeMatchResult ordinata per punteggio di match.</returns>
        public async Task<List<EmployeeMatchResult>> GetBestMatchesForProject(Project project, IEnumerable<Employee> availableEmployees)
        {
            try
            {
                // Prepara i dati per l'AI
                string projectSkillsJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    HardSkills = project.RequiredHardSkills,
                    SoftSkills = project.RequiredSoftSkills
                });

                string employeesJson = System.Text.Json.JsonSerializer.Serialize(availableEmployees.Select(e => new
                {
                    e.Id,
                    e.FirstName,
                    e.LastName,
                    e.HardSkills,
                    e.SoftSkills,
                    e.Experiences // L'AI può usare le esperienze per valutare la rilevanza delle skill
                }));

                string prompt = $@"Sei un analista di talenti AI. Ti fornirò le competenze richieste per un progetto e un elenco di dipendenti disponibili con le loro competenze ed esperienze.
                                    Il tuo compito è:

                                    Valutare il matching di ogni dipendente rispetto alle competenze richieste dal progetto.
                                    Calcolare un punteggio di matching (da 0.0 a 1.0, dove 1.0 è un match perfetto) per ogni dipendente.
                                    Identificare le competenze mancanti per un match perfetto per ogni dipendente.
                                    Identificare eventuali competenze extra rilevanti che il dipendente possiede.
                                    Restituire una lista ordinata dei dipendenti dal più al meno adatto, includendo il punteggio di matching, le competenze mancanti ed extra.
                                    Competenze richieste dal progetto (HardSkills e SoftSkills):
                                    {projectSkillsJson}
                                    ### Dipendenti disponibili:
                                    {employeesJson}

                                    ### Risposta attesa:
                                    Restituisci solo un oggetto JSON valido, con una lista nel seguente formato:
                                    {{ ""result"":
                                        [
                                            {{
                                                ""employeeId"": ""<ID_Dipendente>"",
                                                ""matchScore"": 0.95,
                                                ""missingSkills"": [""Azure Service Bus""],
                                                ""potentialSkills"": [""GraphQL""],
                                                ""employeeDetails"": {{ ... dettagli sintetici del dipendente ... }}
                                            }},
                                            // ... altri dipendenti ...
                                        ]
                                    }}
                                    Considera che le HardSkills con IsMandatory: true e un MinProficiencyLevel elevato dovrebbero avere un peso maggiore nel punteggio. Le SoftSkills sono importanti per la compatibilità del team.";

                var chatOptions = new ChatCompletionOptions()
                {
                    Temperature = 0.5f, // Aumenta leggermente per un po' di creatività nel matching
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                };

                var messages = new List<ChatMessage>()
                {
                    new SystemChatMessage("Sei un AI specializzato nel matching di talenti. Valuta con precisione le competenze dei dipendenti rispetto ai requisiti di progetto e genera un punteggio di adeguatezza."),
                    new UserChatMessage(prompt)
                };

                

                var response = await _chatClient.CompleteChatAsync(messages, chatOptions);
                var chatResponse = response.Value.Content.Last().Text;

                int jsonStart = chatResponse.IndexOf('[');
                int jsonEnd = chatResponse.LastIndexOf(']');

                if (jsonStart >= 0 && jsonEnd >= 0)
                {
                    chatResponse = chatResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                }

                var matchedEmployees = System.Text.Json.JsonSerializer.Deserialize<List<EmployeeMatchResult>>(chatResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return matchedEmployees;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetBestMatchesForProject: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Suggerisce corsi di formazione a un dipendente basandosi sulle sue competenze e sulle skill gap.
        /// </summary>
        /// <param name="employee">Il dipendente per cui suggerire i corsi.</param>
        /// <param name="allTrainings">Tutti i corsi di formazione disponibili.</param>
        /// <returns>Una lista di SkillTraining suggeriti.</returns>
        public async Task<List<SkillTraining>> GetSuggestedTrainings(Employee employee, IEnumerable<SkillTraining> allTrainings)
        {
            try
            {
                // Prepara i dati per l'AI
                string employeeSkillsJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    HardSkills = employee.HardSkills,
                    SoftSkills = employee.SoftSkills,
                    CurrentRole = employee.CurrentRole,
                    Department = employee.Department
                });

                string trainingsJson = System.Text.Json.JsonSerializer.Serialize(allTrainings.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.SkillsDeveloped,
                    t.Level,
                    t.Url
                }));

                string prompt = $@"Sei un consigliere di carriera AI. Ti fornirò il profilo di un dipendente e una lista di corsi di formazione disponibili.
                                    Il tuo compito è:

                                    Analizzare le competenze attuali del dipendente e il suo ruolo.
                                    Suggerire corsi di formazione pertinenti che possano: a. Colmare eventuali skill gap rispetto alle competenze più richieste nel mercato o nel settore dell'azienda (puoi inferirle dalle competenze del dipendente e dalla sua area). b. Migliorare le competenze esistenti del dipendente. c. Supportare la crescita professionale del dipendente nel suo ruolo attuale o verso un ruolo futuro (se possibile inferirlo dal contesto generale o dalla descrizione del dipendente).
                                    Restituire una lista ordinata dei corsi suggeriti, dal più al meno rilevante.
                                    {{employeeSkillsJson}}
                                    {{trainingsJson}}
                                    [
                                        {{
                                            ""id"": ""<ID_Corso>"",
                                            ""title"": ""Corso Avanzato di Kubernetes"",
                                            ""description"": ""Approfondisci la gestione di cluster Kubernetes avanzati."",
                                            ""provider"": ""Cloud Academy"",
                                            ""url"": ""[https://example.com/kubernetes-course](https://example.com/kubernetes-course)"",
                                            ""estimatedDurationHours"": 40,
                                            ""skillsDeveloped"": [""Kubernetes"", ""Docker"", ""CI/CD""],
                                            ""level"": ""Advanced"",
                                            ""cost"": 500
                                        }},
                                        // ... altri corsi suggeriti ...
                                    ]
                                    Focalizzati sui corsi che aggiungono valore significativo al profilo del dipendente e alla sua progressione di carriera.";

                var chatOptions = new ChatCompletionOptions()
                {
                    Temperature = 0.6f, // Un po' più di "creatività" per suggerimenti diversi
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                };

                var messages = new List<ChatMessage>()
                {
                    new SystemChatMessage("Sei un AI che fornisce consigli di carriera e suggerimenti di formazione personalizzati. Aiuta i dipendenti a crescere professionalmente."),
                    new UserChatMessage(prompt)
                };

                var response = await _chatClient.CompleteChatAsync(messages, chatOptions);
                var chatResponse = response.Value.Content.Last().Text;

                var suggestedTrainings = System.Text.Json.JsonSerializer.Deserialize<List<SkillTraining>>(chatResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return suggestedTrainings;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetSuggestedTrainings: {ex.Message}", ex);
            }
        }



    }
}


#region Old
//public class AzureAIService : IAzureAIService

//{

//    private readonly AzureOpenAIClient _blobServiceClient;

//    private readonly ChatClient _chatClient;

//    public AzureAIService(string openAiEndpoint, string openAiKey, string openAiDeployment)

//    {

//        _blobServiceClient = new AzureOpenAIClient(new Uri(openAiEndpoint), new AzureKeyCredential(openAiKey));

//        _chatClient = _blobServiceClient.GetChatClient(openAiDeployment);

//    }



//    public async Task<string> GetCVData(string cvText)

//    {

//        try

//        {

//            string prompt = $@"Sei un assistente AI che analizza curriculum vitae.

//                            Estrai e restituisci in JSON le seguenti informazioni:

//                            - Name

//                            - LastName

//                            - DateOfBirth

//                            - PlaceOfBirth

//                            - Address

//                            - Phone

//                            - Email

//                            - Experiences (Title, Agency, Period)

//                            - Skills (Name, Vote (da 1 a 10 in base al livello di esperienza))



//                            ATTENZIONE!

//                            Le competenze devono essere normalizzate seguendo esclusivamente la seguente lista predefinita. Usa **esattamente questi nomi**. Se trovi sinonimi o varianti, mappale sul nome corretto.



//                            Elenco competenze valide:

//                            ["".NET"", ""C#"", ""Java"", ""JavaScript"", ""React"", ""Angular"", ""Vue.js"", ""Node.js"", ""Python"", ""SQL Server"", ""Cosmos DB"", ""Azure"", ""Azure Functions"", ""Azure DevOps"", ""Azure Service Bus"", ""Docker"", ""Kubernetes"", ""CI/CD"", ""Git"", ""xUnit"", ""Selenium"", ""Cypress"", ""Microservizi"", ""REST API"", ""NoSQL"", ""Blob Storage"", ""Application Insights"", ""Log Analytics""]



//                            Se una competenza non rientra in questa lista, ignorala.



//                            Testo del CV: 



//                            '''{cvText}'''";



//            var chatOptions = new ChatCompletionOptions()

//            {

//                Temperature = 0.2f,

//            };



//            var messages = new List<ChatMessage>() { new UserChatMessage(prompt) };



//            var response = await _chatClient.CompleteChatAsync(messages, chatOptions);

//            var chatResponse = response.Value.Content.Last().Text;



//            int jsonStart = chatResponse.IndexOf('{');

//            int jsonEnd = chatResponse.LastIndexOf('}');



//            string result = string.Empty;



//            if (jsonStart >= 0 && jsonEnd >= 0)

//            {

//                result = chatResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);

//            }



//            return result;

//        }

//        catch (Exception ex)

//        {

//            throw new Exception(ex.Message);

//        }

//    }



//    public async Task<string> GetProjectSkills(string title, string description)

//    {

//        try

//        {

//            string prompt = $@"Sei un assistente AI specializzato in selezione del personale.

//                            Ricevi il titolo e la descrizione di un'offerta di lavoro.

//                            Estrai e restituisci un JSON contenente una lista delle competenze tecniche e trasversali richieste per questo ruolo.



//                            Formato di output:

//                            {{

//                            ""Title"": ""<Titolo>"",

//                            ""Description"": ""<Descrizione>"",

//                            ""Skills"": [""skill1"", ""skill2"", ...]

//                            }}



//                            Titolo del lavoro: {title}

//                            Descrizione: {description}



//                            ATTENZIONE!

//                            Le competenze devono essere normalizzate seguendo esclusivamente la seguente lista predefinita. Usa **esattamente questi nomi**. Se trovi sinonimi o varianti, mappale sul nome corretto.



//                            Elenco competenze valide:

//                            ["".NET"", ""C#"", ""Java"", ""JavaScript"", ""React"", ""Angular"", ""Vue.js"", ""Node.js"", ""Python"", ""SQL Server"", ""Cosmos DB"", ""Azure"", ""Azure Functions"", ""Azure DevOps"", ""Azure Service Bus"", ""Docker"", ""Kubernetes"", ""CI/CD"", ""Git"", ""xUnit"", ""Selenium"", ""Cypress"", ""Microservizi"", ""REST API"", ""NoSQL"", ""Blob Storage"", ""Application Insights"", ""Log Analytics""]



//                            Se una competenza non rientra in questa lista, ignorala.

//                            ";



//            var chatOptions = new ChatCompletionOptions()

//            {

//                Temperature = 0.2f,

//            };



//            var messages = new List<ChatMessage>()

//      {

//        new SystemChatMessage("Sei un esperto di HR e intelligenza artificiale."),

//        new UserChatMessage(prompt)

//      };



//            var response = await _chatClient.CompleteChatAsync(messages, chatOptions);

//            var chatResponse = response.Value.Content.Last().Text;



//            int jsonStart = chatResponse.IndexOf('{');

//            int jsonEnd = chatResponse.LastIndexOf('}');



//            string result = string.Empty;



//            if (jsonStart >= 0 && jsonEnd >= 0)

//            {

//                result = chatResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);

//            }



//            return result;

//        }

//        catch (Exception ex)

//        {

//            throw new Exception(ex.Message);

//        }

//    }

//}
#endregion