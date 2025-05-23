using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace FinanceFunctions.CosmosEntities
{
    public class Resource : CosmosEntityBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("last-name")]
        public string LastName { get; set; }
        [JsonProperty("date-of-birth")]
        public string DateOfBirth { get; set; }
        [JsonProperty("place-of-birth")]
        public string PlaceOfBirth { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("cv-name")]
        public string CVName { get; set; }
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
