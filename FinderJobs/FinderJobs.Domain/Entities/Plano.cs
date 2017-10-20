using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Plano : EntityBase
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public string Periodo { get; set; }
        public bool Pago { get; set; }
        public List<string> Caracteristicas { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
