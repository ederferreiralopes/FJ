using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class UsuarioHabilidade
    {
        public virtual int Id { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Habilidade Habilidade { get; set; }
    }

    public class UsuarioHabilidadeMap : ClassMap<UsuarioHabilidade>
    {
        public UsuarioHabilidadeMap()
        {
            Table("UsuarioHabilidade");

            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            References(x => x.Usuario).Not.Nullable().Column("UsuarioId");
            References(x => x.Habilidade).Not.Nullable().Column("HabilidadeId");
        }
    }
}
