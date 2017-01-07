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
    public class HabilidadeService : ServiceBase<Habilidade>, IHabilidadeService
    {
        private readonly IHabilidadeRepository _habilidadeRepository;

        public HabilidadeService(IHabilidadeRepository habilidadeRepository)
            : base(habilidadeRepository)
        {
            _habilidadeRepository = habilidadeRepository;
        }

        public IEnumerable<Habilidade> BuscarPorNome(string nome)
        {
            return _habilidadeRepository.GetAll().Where(h => h.Nome.IndexOf(nome, StringComparison.OrdinalIgnoreCase) > -1);
        }
    }
}
