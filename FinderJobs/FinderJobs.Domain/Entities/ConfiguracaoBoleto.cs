using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class ConfiguracaoBoleto
    {

        public ConfiguracaoBoleto()
        {
            this.Cedente = new Cedente();
        }

        public virtual int Id { get; set; }
        public virtual short CodigoBanco { get; set; }
        public virtual DateTime Vencimento { get; set; }
        public virtual decimal ValorBoleto { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string CodigoCarteira { get; set; }
        public virtual string CodigoEspecieDocumento { get; set; }
        public virtual bool MostrarCodigoCarteira { get; set; }
        public virtual bool MostrarComprovanteEntrega { get; set; }
        public virtual Cedente Cedente { get; set; }
        public virtual bool Ativo { get; set; }
    }

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
