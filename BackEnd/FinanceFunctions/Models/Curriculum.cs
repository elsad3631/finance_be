using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceFunctions.Models
{
    public class Curriculum
    {
        public string NomeCompleto { get; set; }
        public List<EsperienzaLavorativa> EsperienzeLavorative { get; set; }
        public List<string> Competenze { get; set; }
    }

    public class EsperienzaLavorativa
    {
        public string Titolo { get; set; }
        public string Azienda { get; set; }
        public string Periodo { get; set; }
    }
}
