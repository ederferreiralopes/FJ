using System;
using System.Linq;
using System.Collections.Generic;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace FinderJobs.Infra.Data.Repositories
{
    public class CadastroRepository : RepositoryBaseMongoDb<Cadastro>, ICadastroRepository
    {
        public Dictionary<string, string> GetDashboard(string tipo, DateTime ano)
        {
            var resultado = collection.Aggregate()
               .Match(BsonDocument.Parse("{DataCadastro: {$gte: ISODate('" + ano.Year + "-01-01T00:00:00.000Z'), $lt: ISODate('" + ano.Year + "-12-31T00:00:00.000Z')}}"))               
               .Match(BsonDocument.Parse("{Tipo: '" + tipo + "'}"))               
               .Group(BsonDocument.Parse("{ _id: {$month: '$DataCadastro'}, total: { $sum: 1}}"))
               .Sort(BsonDocument.Parse(" { _id: 1}")).ToListAsync();
            
            var retorno = new Dictionary<string, string>();
            

            foreach (var item in resultado.Result)
            {
                retorno.Add(((Mes)((int)item.Values.ToList()[0] - 1)).ToString(), item.Values.ToList()[1].ToString());
            }

            return retorno;
        }
    }
}
