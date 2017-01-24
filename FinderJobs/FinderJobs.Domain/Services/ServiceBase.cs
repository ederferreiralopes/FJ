
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
            return _repository.Insert(obj);
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

        public bool Delete(TEntity obj)
        {
            return _repository.Delete(obj);
        }

        public IList<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
