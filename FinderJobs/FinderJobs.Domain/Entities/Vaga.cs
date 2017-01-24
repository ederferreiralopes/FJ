using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Vaga : EntityBase
    {        
        public virtual Usuario Empresa { get; set; }
        public virtual DateTime DataCadastro { get; set; }        
        public virtual string Descricao { get; set; }
        public virtual string Cep { get; set; }
        public virtual List<Habilidade> Habilidades { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
