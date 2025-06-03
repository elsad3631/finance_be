using Newtonsoft.Json;
using System;

namespace FinanceFunctions.CosmosEntities
{
    public class CVData
    {
        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("storage_url")] // URL dove è memorizzato il CV (es. Blob Storage)
        public string StorageUrl { get; set; }

        [JsonProperty("upload_date")]
        public DateTime UploadDate { get; set; }

        [JsonProperty("parsed_version")] // Un contatore o ID per la versione del CV parsata
        public int? ParsedVersion { get; set; }
    }
}
