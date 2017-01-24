using FinderJobs.Domain.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Mapping
{
    public class EnderecoMap : ClassMap<Endereco>
    {
        public EnderecoMap()
        {            
            Table("Endereco");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.Cep).Not.Nullable().Column("Cep");
            Map(x => x.Logradouro).Not.Nullable().Column("Logradouro");
            Map(x => x.Numero).Not.Nullable().Column("Numero");
            Map(x => x.Bairro).Not.Nullable().Column("Bairro");
            Map(x => x.Cidade).Not.Nullable().Column("Cidade");
            Map(x => x.UF).Not.Nullable().Column("UF");
            Map(x => x.Ativo).Not.Nullable().Column("Ativo");            
        }
    }
}
