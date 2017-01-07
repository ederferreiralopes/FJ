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

        public Usuario ValidarAcesso(string login, string senha)
        {
            return _usuarioRepository.GetAll().Where(u => u.Login.Equals(login) && u.Senha.Equals(senha)).FirstOrDefault();
        }

        public bool ValidarLogin(string login)
        {
            return _usuarioRepository.GetAll().Where(u => u.Login.Equals(login)).Count() > 0;
        }
    }
}
