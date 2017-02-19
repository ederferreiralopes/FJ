using FinderJobs.Application.Interface;
using FinderJobs.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace FinderJobs.Application
{
    public class AppServiceBase<TEntity> : IAppServiceBase<TEntity> where TEntity : class
    {
        private readonly IServiceBase<TEntity> _serviceBase;

        public AppServiceBase(IServiceBase<TEntity> appServiceBase)
        {
            _serviceBase = appServiceBase;
        }

        public object Insert(TEntity obj)
        {
            return _serviceBase.Insert(obj);
        }

        public TEntity GetById(Guid id)
        {
            return _serviceBase.GetById(id);
        }

        public bool Update(TEntity obj)
        {
            return _serviceBase.Update(obj);
        }

        public bool UpdateByField(Guid id, string campo, string valor)
        {
            return _serviceBase.UpdateByField(id, campo, valor);
        }

        public bool Disable(Guid id)
        {
            return _serviceBase.Disable(id);
        }

        public bool Delete(TEntity obj)
        {
            return _serviceBase.Delete(obj);
        }

        public IList<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate)
        {
            return _serviceBase.SearchFor(predicate);
        }

        public IList<TEntity> GetAll()
        {
            return _serviceBase.GetAll();
        }
    }
}
