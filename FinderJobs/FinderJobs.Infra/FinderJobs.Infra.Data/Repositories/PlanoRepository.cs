
using System;
using System.Collections.Generic;
using System.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Repositories;

namespace FinderJobs.Infra.Data.Repositories
{
    public class PlanoRepository : RepositoryBaseMongoDb<Plano>, IPlanoRepository
    {       
    }
}
