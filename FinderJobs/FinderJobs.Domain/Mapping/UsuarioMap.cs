using FinderJobs.Domain.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Mapping
{
    public class UsuarioMap : ClassMap<Usuario>
    {

        public UsuarioMap()
        {
            Table("Usuario");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.Pago).Not.Nullable().Column("Pago");
            Map(x => x.Anonimo).Not.Nullable().Column("Anonimo");
            Map(x => x.Nome).Not.Nullable().Column("Nome");
            Map(x => x.Tipo).Not.Nullable().Column("Tipo");
            Map(x => x.Email).Not.Nullable().Column("Email");
            Map(x => x.Celular).Column("Celular");
            Map(x => x.CpfCnpj).Not.Nullable().Column("CpfCnpj");
            References(x => x.Endereco).Not.Nullable().Column("EnderecoId");
            Map(x => x.Habilidades).Not.Nullable().Column("Habilidades");
            Map(x => x.DataCadastro).Not.Nullable().Column("DataCadastro");
            Map(x => x.Ativo).Not.Nullable().Column("Ativo");
        }
    }
}
