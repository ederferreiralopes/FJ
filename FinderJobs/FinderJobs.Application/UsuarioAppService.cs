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
    public class UsuarioAppService : AppServiceBase<Usuario>, IUsuarioAppService
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioAppService(IUsuarioService usuarioService)     
            : base(usuarioService)       
        {
            _usuarioService = usuarioService;
        }

        public IEnumerable<Usuario> BuscarPorTipo(string tipo)
        {
            return _usuarioService.BuscarPorTipo(tipo);
        }

        public Usuario ValidarAcesso(string login, string senha)
        {
            return _usuarioService.ValidarAcesso(login, senha);
        }

        public bool ValidarLogin(string login)
        {
            return _usuarioService.ValidarLogin(login);
        }
    }
}
