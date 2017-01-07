using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Habilidade
    {
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
    }

    public class HabilidadeMap : ClassMap<Habilidade>
    {
        public HabilidadeMap()
        {
            Table("Habilidade");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.Nome).Not.Nullable().Column("Nome");
        }
    }
}
