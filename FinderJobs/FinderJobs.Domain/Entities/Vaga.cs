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
        public Guid EmpresaId { get; set; }
        public DateTime DataCadastro { get; set; }        
        public string Descricao { get; set; }
        public string Cep { get; set; }
        public List<Habilidade> Habilidades { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
