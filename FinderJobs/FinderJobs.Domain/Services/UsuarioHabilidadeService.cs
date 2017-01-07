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
    public class UsuarioHabilidadeService : ServiceBase<UsuarioHabilidade>, IUsuarioHabilidadeService
    {
        private readonly IUsuarioHabilidadeRepository _usuarioHabilidadeRepository;

        public UsuarioHabilidadeService(IUsuarioHabilidadeRepository usuarioHabilidadeRepository)
            : base(usuarioHabilidadeRepository)
        {
            _usuarioHabilidadeRepository = usuarioHabilidadeRepository;
        }

        public IEnumerable<UsuarioHabilidade> GetByUserId(int id)
        {
            return _usuarioHabilidadeRepository.GetAll().Where(uh => uh.Usuario.Id == id);
        }
    }
}
