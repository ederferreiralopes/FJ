using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Interfaces.Repositories
{

    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        object Insert(TEntity entity);
        bool Update(TEntity entity);
        bool UpdateByField(Guid id, string campo, string valor);
        bool Disable(Guid id);
        bool Delete(TEntity entity);
        IList<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate);
        IList<TEntity> GetAll();
        IList<TEntity> Find(string query, int pagina);             
        TEntity GetById(Guid id);
    }

    public interface IRepositoryBaseNhibernate<TEntity> where TEntity : class
    {
        object Add(TEntity obj);
        TEntity GetById(int id);
        IEnumerable<TEntity> GetAll();
        void Update(TEntity obj);
        void Remove(TEntity obj);
        void Dispose();
    }
}
