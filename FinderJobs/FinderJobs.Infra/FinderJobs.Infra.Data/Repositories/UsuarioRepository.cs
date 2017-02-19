
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Infra.Data.Context;
using FinderJobs.Domain.Interfaces.Repositories;

namespace FinderJobs.Infra.Data
{
    public class UsuarioRepository : RepositoryBaseMongoDb<Usuario>, IUsuarioRepository
    {
        //public IList<Usuario> BuscarPorTipo(string tipo)
        //{
        //    return Find("{ Tipo : '" + tipo + "'}");
        //}

        //public IList<Usuario> BuscarPorTipo(string tipo, List<Habilidade> habilidades)
        //{
        //    var query = "{ Tipo : '" + tipo + "'}";
        //    if (habilidades != null && habilidades.Count > 0)
        //    {
        //        query = "{ Tipo : '" + tipo + "', 'Habilidades' : { $elemMatch: {";
        //        foreach (var item in habilidades)
        //        {
        //            query += "Nome : '" + item.Nome + "'";
        //        }

        //        query += query + "'}}}";
        //    }

        //    return Find(query);
        //}
    }
}
