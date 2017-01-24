using FluentNHibernate.Mapping;
using System.Collections.Generic;

namespace FinderJobs.Domain.Entities
{
    public class Usuario : EntityBase
    {        
        public virtual bool Pago { get; set; }
        public virtual bool Anonimo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Tipo { get; set; }
        public virtual string Email { get; set; }
        public virtual string Celular { get; set; }
        public virtual string CpfCnpj { get; set; }
        public virtual string DataCadastro { get; set; }
        public virtual string DataAlteracao { get; set; }
        public virtual List<Habilidade> Habilidades { get; set; }
        public virtual List<Arquivo> Arquivos { get; set; }
        public virtual Endereco Endereco { get; set; }        
    }
}