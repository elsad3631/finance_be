using Newtonsoft.Json;
using System.Collections.Generic;

namespace FinanceFunctions.CosmosEntities
{
    public class Resource : CosmosEntityBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("last-name")]
        public string LastName { get; set; }
        [JsonProperty("experiences")]
        public List<Experience> Experiences { get; set; }
        [JsonProperty("skills")]
        public List<Skills> Skills { get; set; }
    }

    public class Experience
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("agency")]
        public string Agency { get; set; }
        [JsonProperty("period")]
        public string Period { get; set; }
    }

    public class Skills
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("vote")]
        public int Vote { get; set; }
    }
}
