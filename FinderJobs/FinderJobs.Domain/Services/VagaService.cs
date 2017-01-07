using FinderJobs.Domain.Entities;
using FinderJobs.Domain.Interfaces.Repositories;
using FinderJobs.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Services
{
    public class VagaService : ServiceBase<Vaga>, IVagaService
    {
        private readonly IVagaRepository _vagaRepository;

        public VagaService(IVagaRepository vagaRepository)
            : base(vagaRepository)
        {
            _vagaRepository = vagaRepository;
        }

        public IEnumerable<Vaga> BuscarPorEmpresa(int id)
        {
            return _vagaRepository.GetAll().Where(v => v.Empresa.Id == id);
        }
    }
}
