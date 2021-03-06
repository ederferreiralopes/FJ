﻿using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Interfaces.Repositories
{
    public interface IHabilidadeRepository : IRepositoryBase<Habilidade>
    {
        Dictionary<string, string> GetDashboard();
    }
}
