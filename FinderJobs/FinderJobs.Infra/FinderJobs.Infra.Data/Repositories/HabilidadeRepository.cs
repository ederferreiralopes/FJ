using System.Collections.Generic;
using System.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Domain.Interfaces.Repositories;
using System;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace FinderJobs.Infra.Data.Repositories
{
    public class HabilidadeRepository : RepositoryBaseMongoDb<Habilidade>, IHabilidadeRepository
    {
        public Dictionary<string, string> GetDashboard()
        {
            var resultado = collection.Aggregate()
               .Group(BsonDocument.Parse("{ _id : '$Ativo', total : { $sum : 1}}")).ToListAsync();

            var retorno = new Dictionary<string, string>();

            foreach (var item in resultado.Result)
            {
                retorno.Add(item.Values.ToList()[0].ToString(), item.Values.ToList()[1].ToString());
            }

            return retorno;
        }
    }
}
