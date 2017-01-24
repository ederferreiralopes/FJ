using FinderJobs.Domain.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Mapping
{
    public class ArquivoMap : ClassMap<Arquivo>
    {
        public ArquivoMap()
        {
            Table("Arquivo");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            References(x => x.Usuario).Not.Nullable().Column("UsuarioId");
            Map(x => x.Caminho).Not.Nullable().Column("Caminho");
            Map(x => x.Tipo).Not.Nullable().Column("Tipo");
            Map(x => x.Nome).Not.Nullable().Column("Nome");
            Map(x => x.Ativo).Not.Nullable().Column("Ativo");
        }
    }
}
