using System.Text.Json.Serialization;

namespace FinanceFunctions.Models.InputModels
{
    public class CVUploadInput
    {
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        [JsonPropertyName("cvContentBase64")]
        public string CVContentBase64 { get; set; } // Per inviare il contenuto del CV in Base64
        [JsonPropertyName("cvStorageUrl")]
        public string CVStorageUrl { get; set; } // O un URL pre-caricato
    }
}
