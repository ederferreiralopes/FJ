using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Arquivo
    {
        public virtual int Id { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual string Caminho { get; set; }
        public virtual string Tipo { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Ativo { get; set; }            
    }

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
