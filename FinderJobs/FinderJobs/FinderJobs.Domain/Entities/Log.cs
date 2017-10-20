using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public class Log: EntityBase
    {
        public string Tipo { get; set; }
        public string Mensagem { get; set; }
        public object Objeto { get; set; }
        public DateTime Data { get; set; }
    }
}
