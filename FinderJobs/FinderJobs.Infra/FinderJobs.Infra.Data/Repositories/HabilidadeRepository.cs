
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Infra.Data.Repositories;
using FinderJobs.Infra.Data.Context;
using FinderJobs.Domain.Interfaces.Repositories;
using System;
using System.Linq.Expressions;

namespace FinderJobs.Infra.Data
{
    public class HabilidadeRepository : RepositoryBaseMongoDb<Habilidade>, IHabilidadeRepository
    {
    }
}
