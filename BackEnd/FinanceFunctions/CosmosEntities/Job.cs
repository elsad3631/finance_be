using System.Collections.Generic;

namespace FinanceFunctions.CosmosEntities
{
    public class Job: CosmosEntityBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Skills { get; set; }
    }
}
