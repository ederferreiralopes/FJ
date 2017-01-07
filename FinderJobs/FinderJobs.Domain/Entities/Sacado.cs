using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinderJobs.Domain.Entities
{
    public class Sacado
    {
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual string CpfCnpj { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string Cep { get; set; }
        public virtual string Uf { get; set; }
    }

    public class SacadoMap : ClassMap<Sacado>
    {
        public SacadoMap()
        {
            Table("Sacado");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.Nome).Not.Nullable().Column("Nome");
            Map(x => x.CpfCnpj).Not.Nullable().Column("CpfCnpj");
            Map(x => x.Endereco).Not.Nullable().Column("Endereco");
            Map(x => x.Bairro).Not.Nullable().Column("Bairro");
            Map(x => x.Cidade).Not.Nullable().Column("Cidade");
            Map(x => x.Cep).Not.Nullable().Column("Cep");
            Map(x => x.Uf).Not.Nullable().Column("Uf");
        }
    }
}
