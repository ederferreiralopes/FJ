using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinderJobs.Domain.Entities
{
    public class Cedente : EntityBase
    {        
        public string Codigo { get; set; }
        public string NossoNumero { get; set; }
        public string CpfCnpj { get; set; }
        public string Nome { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string DigitoConta { get; set; }        
    }   
}
