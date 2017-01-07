using FinderJobs.Application.Interface;
using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Application
{
    public class VagaAppService : AppServiceBase<Vaga>, IVagaAppService
    {
        private readonly IVagaService _vagaService;

        public VagaAppService(IVagaService vagaService)     
            : base(vagaService)       
        {
            _vagaService = vagaService;
        }

        public IEnumerable<Vaga> BuscarPorEmpresa(int id)
        {
            return _vagaService.BuscarPorEmpresa(id);
        }
    }
}
