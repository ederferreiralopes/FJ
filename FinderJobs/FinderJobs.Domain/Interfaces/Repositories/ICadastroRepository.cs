﻿using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Interfaces.Repositories
{
    public interface ICadastroRepository : IRepositoryBase<Cadastro>
    {
        Dictionary<string, string> GetDashboard(string tipo, DateTime ano);
    }
}
