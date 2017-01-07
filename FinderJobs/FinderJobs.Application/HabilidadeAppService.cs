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
    public class HabilidadeAppService : AppServiceBase<Habilidade>, IHabilidadeAppService
    {
        private readonly IHabilidadeService _habilidadeService;

        public HabilidadeAppService(IHabilidadeService habilidadeService)     
            : base(habilidadeService)       
        {
            _habilidadeService = habilidadeService;
        }

        public IEnumerable<Habilidade> BuscarPorNome(string nome)
        {
            return _habilidadeService.BuscarPorNome(nome);
        }
    }
}
