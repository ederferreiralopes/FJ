using FinderJobs.Infra.Data.Context;
using FinderJobs.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Infra.Data.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
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
