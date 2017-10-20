using FinderJobs.Domain.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Mapping
{
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
            Map(x => x.UF).Not.Nullable().Column("UF");
        }
    }
}
