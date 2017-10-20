using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Entities
{
    public abstract class EntityBase
    {
        [BsonId]
        public Guid Id { get; set; }
        public bool Ativo { get; set; }
    }
}
