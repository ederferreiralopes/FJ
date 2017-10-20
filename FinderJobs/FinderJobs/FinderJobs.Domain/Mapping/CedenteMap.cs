using FinderJobs.Domain.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Mapping
{
    public class CedenteMap : ClassMap<Cedente>
    {
        public CedenteMap()
        {
            Table("Cedente");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.Codigo).Not.Nullable().Column("Codigo");
            Map(x => x.NossoNumero).Not.Nullable().Column("NossoNumero");
            Map(x => x.CpfCnpj).Not.Nullable().Column("CpfCnpj");
            Map(x => x.Nome).Not.Nullable().Column("Nome");
            Map(x => x.Agencia).Not.Nullable().Column("Agencia");
            Map(x => x.Conta).Not.Nullable().Column("Conta");
            Map(x => x.DigitoConta).Not.Nullable().Column("DigitoConta");
            Map(x => x.Ativo).Column("Ativo");
        }
    }
}
