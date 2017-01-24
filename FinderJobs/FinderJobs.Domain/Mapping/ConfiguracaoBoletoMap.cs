using FinderJobs.Domain.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Mapping
{
    public class ConfiguracaoBoletoMap : ClassMap<ConfiguracaoBoleto>
    {
        public ConfiguracaoBoletoMap()
        {
            Table("ConfiguracaoBoleto");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.CodigoBanco).Column("CodigoBanco");
            Map(x => x.Vencimento).Column("Vencimento");
            Map(x => x.ValorBoleto).Column("ValorBoleto");
            Map(x => x.NumeroDocumento).Column("NumeroDocumento");
            Map(x => x.Descricao).Column("Descricao");
            Map(x => x.CodigoCarteira).Column("CodigoCarteira");
            Map(x => x.CodigoEspecieDocumento).Column("CodigoEspecieDocumento");
            Map(x => x.MostrarCodigoCarteira).Column("MostrarCodigoCarteira");
            Map(x => x.MostrarComprovanteEntrega).Not.Nullable().Column("MostrarComprovanteEntrega");
            References(x => x.Cedente).Not.Nullable().Not.LazyLoad().Column("CedenteId");
            Map(x => x.Ativo).Not.Nullable().Column("Ativo");
        }
    }
}
