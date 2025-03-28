namespace BackEnd.Models.CustomerModels
{
    public class CustomerUpdateModel
    {
        public int Id { get; set; }
        public string Code { get; set; } // - CODICE
        public string Name { get; set; } // – NOME/RAGIONE SOCIALE
        public int? CustomerTypeId { get; set; } // TIPOLOGIA CLIENTE
        public string? Address { get; set; } // – INDIRIZZO
        public string? ZipCode { get; set; } // – CAP
        public string? City { get; set; } // – CITTA’
        public string? Country { get; set; } // – NAZIONE
        public string? Province { get; set; } // – PROVINCIA
        public string? AdministrativeOfficeAddress { get; set; } // – INDIRIZZO SEDE AMMINISTRATIVA
        public string? AdministrativeOfficeZipCode { get; set; } // – CAP SEDE AMMINISTRATIVA
        public string? AdministrativeOfficeCity { get; set; } // – CITTA’ SEDE AMMINISTRATIVA
        public string? AdministrativeOfficeCountry { get; set; } // – NAZIONE SEDE AMMINISTRATIVA
        public string? AdministrativeOfficeProvince { get; set; } // – PROVINCIA SEDE AMMINISTRATIVA
        public string? OperatingOfficeAddress { get; set; } // – INDIRIZZO
        public string? OperatingOfficeZipCode { get; set; } // – CAP
        public string? OperatingOfficeCity { get; set; } // – CITTA’
        public string? OperatingOfficeCountry { get; set; } // – NAZIONE
        public string? OperatingOfficeProvince { get; set; } // – PROVINCIA
        public string? VATNumber { get; set; } // - P.IVA
        public string? FiscalCode { get; set; } // - CODICE FISCALE
        public string? ContactPerson { get; set; } // REFERENTE
        public string? Phone { get; set; } // – TELEFONO
        public string? Mobile { get; set; } // – CELLULARE
        public string Email { get; set; } // - EMAIL
        public string? PEC { get; set; } // – PEC
        public string? UniqueCode { get; set; } // CODICE UNIVOCO
        public string? ReferenceAgent { get; set; }
        public int? PaymentTypeId { get; set; } // – MODALITA’ DI PAGAMENTO
        public string? BankCoordinates { get; set; } // – COORDINATE BANCARIE
        public string? Notes { get; set; } // – NOTE
        public string? Fax { get; set; } // – FAX
        public string? Bank { get; set; } // – BANCA
        public int? DeliveryTypeId { get; set; } // – ID TIPOLOGIACONSEGNA
        public DateTime UpdateDate { get; set; }
    }
}
