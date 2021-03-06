﻿using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Application.Interface
{
    public interface IVagaAppService : IAppServiceBase<Vaga>
    {
        IEnumerable<Vaga> BuscarPorEmpresa(Guid Id);
        IEnumerable<Vaga> BuscarPaginadaPorEmpresa(Guid Id, int pagina);
        IEnumerable<Vaga> BuscarVagas(List<string> habilidades, int pagina);
    }
}
