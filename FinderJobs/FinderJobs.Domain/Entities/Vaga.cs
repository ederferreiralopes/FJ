using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Vaga
    {
        public virtual int Id { get; set; }
        public virtual Usuario Empresa { get; set; }
        public virtual DateTime DataCadastro { get; set; }        
        public virtual string Descricao { get; set; }
        public virtual string Cep { get; set; }
        public virtual string Habilidades { get; set; }
        public virtual bool Ativo { get; set; }
    }

    public class VagaMap : ClassMap<Vaga>
    {
        public VagaMap()
        {
            Table("Vaga");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            References(x => x.Empresa).Not.LazyLoad().Not.Nullable().Column("EmpresaId");
            Map(x => x.DataCadastro).Not.Nullable().Column("DataCadastro");         
            Map(x => x.Descricao).Not.Nullable().Column("Descricao");
            Map(x => x.Cep).Not.Nullable().Column("Cep");
            Map(x => x.Habilidades).Not.Nullable().Column("Habilidades");
            Map(x => x.Ativo).Not.Nullable().Column("Ativo");
        }
    }
}
