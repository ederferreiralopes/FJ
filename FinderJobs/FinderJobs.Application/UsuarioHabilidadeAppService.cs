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
    public class UsuarioHabilidadeAppService : AppServiceBase<UsuarioHabilidade>, IUsuarioHabilidadeAppService
    {
        private readonly IUsuarioHabilidadeService _usuarioHabilidadeService;

        public UsuarioHabilidadeAppService(IUsuarioHabilidadeService usuarioHabilidadeService)     
            : base(usuarioHabilidadeService)       
        {
            _usuarioHabilidadeService = usuarioHabilidadeService;
        }

        public IEnumerable<UsuarioHabilidade> GetByUserId(int id)
        {
            return _usuarioHabilidadeService.GetByUserId(id);
        }
    }
}
