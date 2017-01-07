using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinderJobs.Domain.Entities
{
    public class Cedente
    {
        public virtual int Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string NossoNumero { get; set; }
        public virtual string CpfCnpj { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Agencia { get; set; }
        public virtual string Conta { get; set; }
        public virtual string DigitoConta { get; set; }
        public virtual bool Ativo { get; set; }
    }

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
