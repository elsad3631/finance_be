using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinanceFunctions.CosmosEntities
{
    public class BraveSearch: CosmosEntityBase
    {
        [JsonPropertyName("news")]
        public List<NewsSection> News { get; set; }
    }

    public class NewsSection
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("results")]
        public List<NewsItem> Results { get; set; }
    }

    public class NewsItem
    {
        [JsonProperty("news_id")]
        public string NewsId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("is_source_local")]
        public bool IsSourceLocal { get; set; }

        [JsonPropertyName("is_source_both")]
        public bool IsSourceBoth { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("page_age")]
        public string PageAge { get; set; }

        [JsonPropertyName("family_friendly")]
        public bool FamilyFriendly { get; set; }

        //[JsonPropertyName("meta_url")]
        //public MetaUrl MetaUrl { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("breaking")]
        public bool Breaking { get; set; }

        [JsonPropertyName("is_live")]
        public bool IsLive { get; set; }

        //[JsonPropertyName("thumbnail")]
        //public Thumbnail Thumbnail { get; set; }

        [JsonPropertyName("age")]
        public string Age { get; set; }

        [JsonPropertyName("extra_snippets")]
        public List<string> ExtraSnippets { get; set; }
    }

    public class MetaUrl
    {
        [JsonPropertyName("scheme")]
        public string Scheme { get; set; }

        [JsonPropertyName("netloc")]
        public string Netloc { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("favicon")]
        public string Favicon { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
    }

    public class Thumbnail
    {
        [JsonPropertyName("src")]
        public string Src { get; set; }

        [JsonPropertyName("original")]
        public string Original { get; set; }
    }
}
