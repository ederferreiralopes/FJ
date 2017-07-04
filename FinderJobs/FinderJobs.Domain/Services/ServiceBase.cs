
using System;
using System.Collections.Generic;
using FinderJobs.Domain.Interfaces.Services;
using FinderJobs.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace FinderJobs.Domain.Services
{
    public class ServiceBase<TEntity> : IServiceBase<TEntity> where TEntity : class
    {

        private readonly IRepositoryBase<TEntity> _repository;

        public ServiceBase(IRepositoryBase<TEntity> repository)
        {
            _repository = repository;
        }

        public object Insert(TEntity obj)
        {
            try
            {
                var resultado =_repository.Insert(obj);
                return resultado;
            }
            catch (Exception e)
            {
                throw  e;
            }            
        }

        IList<TEntity> IServiceBase<TEntity>.GetAll()
        {
            return _repository.GetAll();
        }

        public TEntity GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public bool Update(TEntity obj)
        {
            return _repository.Update(obj);
        }

        public bool UpdateByField(Guid id, string campo, string valor)
        {
            return _repository.UpdateByField(id, campo, valor);
        }

        public bool Disable(Guid id)
        {
            return _repository.Disable(id);
        }

        public bool Delete(TEntity obj)
        {
            return _repository.Delete(obj);
        }

        public IList<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.SearchFor(predicate);
        }

        public IList<TEntity> Find(string query, int pagina)
        {
            return _repository.Find(query, pagina);
        }
    }
}
