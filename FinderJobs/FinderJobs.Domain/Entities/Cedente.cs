using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinderJobs.Domain.Entities
{
    public class Cedente : EntityBase
    {        
        public virtual string Codigo { get; set; }
        public virtual string NossoNumero { get; set; }
        public virtual string CpfCnpj { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Agencia { get; set; }
        public virtual string Conta { get; set; }
        public virtual string DigitoConta { get; set; }        
    }   
}
