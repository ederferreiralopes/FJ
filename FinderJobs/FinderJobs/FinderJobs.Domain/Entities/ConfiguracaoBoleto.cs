
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class ConfiguracaoBoleto : EntityBase
    {

        public ConfiguracaoBoleto()
        {
            this.Cedente = new Cedente();
        }
        
        public short CodigoBanco { get; set; }
        public DateTime Vencimento { get; set; }
        public decimal ValorBoleto { get; set; }
        public string NumeroDocumento { get; set; }
        public string Descricao { get; set; }
        public string CodigoCarteira { get; set; }
        public string CodigoEspecieDocumento { get; set; }
        public bool MostrarCodigoCarteira { get; set; }
        public bool MostrarComprovanteEntrega { get; set; }
        public Cedente Cedente { get; set; }        
    }   
}
