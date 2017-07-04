using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Pagamento : EntityBase
    {
        public string Codigo { get; set; }
        public string Referencia { get; set; }
        public Plano Plano { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
