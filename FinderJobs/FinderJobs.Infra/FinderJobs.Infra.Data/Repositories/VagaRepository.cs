
using System.Collections.Generic;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Domain.Interfaces.Repositories;

namespace FinderJobs.Infra.Data.Repositories
{
    public class VagaRepository : RepositoryBaseMongoDb<Vaga>, IVagaRepository
    {
        public List<Vaga> BuscarVagas()
        {
            return null;
        }

        public List<Vaga> BuscarPorEmpresa(int idEmpresa)
        {
            return null;
        }
    }
}
