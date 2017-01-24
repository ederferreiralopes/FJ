
using System.Collections.Generic;
using System.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Repositories;
using FinderJobs.Infra.Data.Context;
using System;
using MongoDB.Driver;
using MongoDB.Bson;

namespace FinderJobs.Infra.Data.Repositories
{
    public class ArquivoRepository : RepositoryBaseMongoDb<Arquivo>, IArquivoRepository
    {
        public void Desativar(Guid id)
        {
            //var context = MongoDbContext.Create();
            //var filter = Builders<Arquivo>.Filter.Eq("_i", id);
            //var update = Builders<Arquivo>.Update.Set("Ativo", false); //.CurrentDate("lastModified");
            //context.Arquivo.UpdateMany(filter, update);
        }

        public void Desativar(Guid usuarioId, string tipo)
        {
            //var context = MongoDbContext.Create();
            //var filter = Builders<Arquivo>.Filter.Eq("UsuarioId", usuarioId) & Builders<Arquivo>.Filter.Eq("Tipo", tipo);
            //var update = Builders<Arquivo>.Update.Set("Ativo", false); //.CurrentDate("lastModified");
            //context.Arquivo.UpdateMany(filter, update);
        }        
    }
}
