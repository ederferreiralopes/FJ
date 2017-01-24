using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Arquivo : EntityBase
    {        
        public virtual Usuario Usuario { get; set; }
        public virtual string Caminho { get; set; }
        public virtual string Tipo { get; set; }
        public virtual string Nome { get; set; }               
    }
}
