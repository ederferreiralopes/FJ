using System;
using System.Linq;
using System.Collections.Generic;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinderJobs.Infra.Data
{
    public class UsuarioRepository : RepositoryBaseMongoDb<Usuario>, IUsuarioRepository
    {
        public Dictionary<string, string> GetDashboard(string tipo, DateTime inicio, DateTime fim)
        {
            var resultado = collection.Aggregate()               
               .Match(BsonDocument.Parse("{DataCadastro: {$gte: ISODate('2017-01-01T00:00:00.000Z'), $lt: ISODate('2017-12-31T00:00:00.000Z')}}"))               
               .Match(BsonDocument.Parse("{Tipo: '" + tipo + "'}"))               
               .Group(BsonDocument.Parse("{ _id: {$month: '$DataCadastro'}, total: { $sum: 1}}"))
               .Sort(BsonDocument.Parse(" { _id: 1}"))
               .ToList();
            
            var retorno = new Dictionary<string, string>();
            

            foreach (var item in resultado)
            {
                retorno.Add(((Mes)((int)item.Values.ToList()[0] - 1)).ToString(), item.Values.ToList()[1].ToString());
            }

            return retorno;
        }
    }
}
