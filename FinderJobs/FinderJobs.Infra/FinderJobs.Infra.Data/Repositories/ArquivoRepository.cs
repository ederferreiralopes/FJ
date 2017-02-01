
using System.Collections.Generic;
using System.Linq;
using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Repositories;
using FinderJobs.Infra.Data.Context;
using System;
using MongoDB.Driver;
using MongoDB.Bson;

namespace FinderJobs.Infra.Data.Repositories
{
    public class ArquivoRepository : RepositoryBaseMongoDb<Arquivo>, IArquivoRepository
    {       
    }
}
