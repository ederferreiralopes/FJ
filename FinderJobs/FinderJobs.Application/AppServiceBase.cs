using FinderJobs.Application.Interface;
using FinderJobs.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FinderJobs.Infra.CrossCutting;

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
            try
            {
                return _serviceBase.Insert(obj);                                
            }
            catch (Exception erro)
            {
                LogService.NotifyException("Insert", erro);                
                return new { erro = erro.Message };
            }            
        }

        public TEntity GetById(Guid id)
        {
            try
            {
                return _serviceBase.GetById(id);
            }
            catch (Exception erro)
            {
                LogService.NotifyException("Insert", erro);
                return null;
            }            
        }

        public bool Update(TEntity obj)
        {

            try
            {
                return _serviceBase.Update(obj);
            }
            catch (Exception erro)
            {
                LogService.NotifyException("Insert", erro);
                return false;
            }
        }

        public bool UpdateByField(Guid id, string campo, string valor)
        {            
            try
            {
                return _serviceBase.UpdateByField(id, campo, valor);
            }
            catch (Exception erro)
            {
                LogService.NotifyException("Insert", erro);
                return false;
            }
        }

        public bool Disable(Guid id)
        {            
            try
            {
                return _serviceBase.Disable(id);
            }
            catch (Exception erro)
            {
                LogService.NotifyException("Insert", erro);
                return false;
            }
        }

        public bool Delete(TEntity obj)
        {            
            try
            {
                return _serviceBase.Delete(obj);
            }
            catch (Exception erro)
            {
                LogService.NotifyException("Insert", erro);
                return false;
            }
        }

        public IList<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate)
        {            
            try
            {
                return _serviceBase.SearchFor(predicate);
            }
            catch (Exception erro)
            {
                LogService.NotifyException("Insert", erro);
                return null;
            }
        }

        public IList<TEntity> GetAll()
        {
            try
            {
                return _serviceBase.GetAll();
            }
            catch (Exception erro)
            {
                LogService.NotifyException("Insert", erro);
                return null;
            }            
        }
    }
}
