using FinderJobs.Infra.Data.Context;
using FinderJobs.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Linq.Expressions;
using FinderJobs.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace FinderJobs.Infra.Data.Repositories
{
    public class RepositoryBaseMongoDb<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
    {
        private IMongoDatabase database;
        public IMongoCollection<TEntity> collection;

        public RepositoryBaseMongoDb()
        {
            GetDatabase();
            GetCollection();
        }

        public object Insert(TEntity entity)
        {            
            entity.Id = Guid.NewGuid();
            entity.Ativo = true;
            collection.InsertOne(entity);
            return entity.Id;     
        }

        public bool Update(TEntity entity)
        {
            return collection.ReplaceOne(Builders<TEntity>.Filter.Eq("_id", entity.Id), entity).ModifiedCount > 0;
        }

        public bool UpdateByField(Guid id, string campo, string valor)
        {
            var resultado = collection.UpdateOne(Builders<TEntity>.Filter.Eq("_id", id), Builders<TEntity>.Update.Set(campo, valor));
            return resultado.ModifiedCount > 0;
        }

        public bool Disable(Guid id)
        {
            return collection.UpdateOne(Builders<TEntity>.Filter.Eq("_id", id), Builders<TEntity>.Update.Set("Ativo", false)).ModifiedCount > 0;
        }

        public bool Delete(TEntity entity)
        {
            return collection.DeleteOne(Builders<TEntity>.Filter.Eq("_id", entity.Id)).DeletedCount > 0;
        }

        public IList<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate)
        {
            return collection.AsQueryable<TEntity>().Where(predicate.Compile()).ToList();
        }

        public IList<TEntity> GetAll()
        {
            return collection.Find<TEntity>(_ => true).ToList();
        }

        public IList<TEntity> Find(string query, int pagina)
        {
            return collection.Find<TEntity>(query).Skip(pagina * 10).Limit(10).ToList();
        }

        public TEntity GetById(Guid id)
        {
            return collection.Find<TEntity>(Builders<TEntity>.Filter.Eq("_id", id)).FirstOrDefault();
        }

        #region Private Helper Methods
        private void GetDatabase()
        {
            var client = new MongoClient(GetConnectionString());
            database = client.GetDatabase(GetDatabaseName());
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.AppSettings.Get("MongoDbConnectionString").Replace("{DB_NAME}", GetDatabaseName());
        }

        private string GetDatabaseName()
        {
            return ConfigurationManager.AppSettings.Get("MongoDbDatabaseName");
        }

        public void GetCollection()
        {
            collection = database.GetCollection<TEntity>(typeof(TEntity).Name);
        }
        #endregion
    }

    public class RepositoryBaseNhibernate<TEntity> : IRepositoryBaseNhibernate<TEntity> where TEntity : class
    {
        /// <summary>
        /// Método para inserir
        /// </summary>
        /// <param name="obj"></param>        
        public object Add(TEntity obj)
        {
            object entidade = null;
            using (ISession session = SessionFactory.AbrirSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        entidade = session.Save(obj);
                        transaction.Commit();
                    }
                    catch (Exception erro)
                    {
                        if (!transaction.WasCommitted)
                        {
                            transaction.Rollback();
                        }
                        throw new Exception("Erro ao inserir dados : " + erro.Message);
                    }
                }
            }

            return entidade;
        }

        public void Update(TEntity obj)
        {
            using (ISession session = SessionFactory.AbrirSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Update(obj);
                        transaction.Commit();
                    }
                    catch (Exception erro)
                    {
                        if (!transaction.WasCommitted)
                        {
                            transaction.Rollback();
                        }
                        throw new Exception("Erro ao alterar dados : " + erro.Message);
                    }
                }
            }
        }

        public void Remove(TEntity obj)
        {
            using (ISession session = SessionFactory.AbrirSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Delete(obj);
                        transaction.Commit();
                    }
                    catch (Exception erro)
                    {
                        if (!transaction.WasCommitted)
                        {
                            transaction.Rollback();
                        }
                        throw new Exception("Erro ao excluir dados : " + erro.Message);
                    }
                }
            }
        }        

        public TEntity GetById(int Id)
        {
            using (ISession session = SessionFactory.AbrirSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    return session.Get<TEntity>(Id);
                }
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            using (ISession session = SessionFactory.AbrirSession())
            {
                using (ITransaction transacao = session.BeginTransaction())
                {
                    return (from c in session.Query<TEntity>() select c).ToList();
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
