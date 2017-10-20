using FinderJobs.Domain.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Mapping
{
    public class HabilidadeMap : ClassMap<Habilidade>
    {
        public HabilidadeMap()
        {
            Table("Habilidade");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.Nome).Not.Nullable().Column("Nome");
            Map(x => x.Ativo).Not.Nullable().Column("Ativo");
        }
    }
}
