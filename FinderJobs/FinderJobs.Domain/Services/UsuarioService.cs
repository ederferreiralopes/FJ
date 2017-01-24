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
    public class UsuarioService : ServiceBase<Usuario>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
            : base(usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public IEnumerable<Usuario> BuscarPorTipo(string tipo)
        {
            return _usuarioRepository.GetAll().Where(u => u.Tipo.Equals(tipo));
        }

        public Usuario GetByEmail(string email)
        {
            //return _usuarioRepository.GetAll().Where(u => u.Email.Equals(email)).FirstOrDefault();
            return _usuarioRepository.SearchFor(u => u.Email == email).FirstOrDefault();
        }
    }
}
