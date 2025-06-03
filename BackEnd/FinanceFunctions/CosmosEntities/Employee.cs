using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace FinanceFunctions.CosmosEntities
{
    public class Employee : CosmosEntityBase
    {
        [JsonProperty("first_name")] // Changed from name to first_name for clarity
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("date_of_birth")]
        public DateTime? DateOfBirth { get; set; } // Changed to DateTime?

        [JsonProperty("place_of_birth")]
        public string PlaceOfBirth { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("cv_data")] // Container per i dati del CV, inclusi il nome del file e l'URL
        public CVData CVData { get; set; } // Vedere nuova entità CVData

        [JsonProperty("experiences")]
        public List<Experience> Experiences { get; set; }

        [JsonProperty("hard_skills")] // Skills hard possedute
        public List<EmployeeSkill> HardSkills { get; set; } // Vedere modifica a Skills (ora EmployeeSkill)

        [JsonProperty("soft_skills")] // Soft skills possedute
        public List<EmployeeSkill> SoftSkills { get; set; } // Puoi usare la stessa struttura per soft skills

        [JsonProperty("current_role")] // Ruolo attuale nell'azienda
        public string CurrentRole { get; set; }

        [JsonProperty("department")] // Dipartimento di appartenenza
        public string Department { get; set; }

        [JsonProperty("is_available")] // Indica se il dipendente è disponibile per nuovi progetti
        public bool IsAvailable { get; set; } = true;

        [JsonProperty("user_id")] // Se hai un sistema di autenticazione, collega all'ID utente
        public string UserId { get; set; }
    }
}
