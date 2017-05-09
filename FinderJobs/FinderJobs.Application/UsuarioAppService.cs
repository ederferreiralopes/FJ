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

        public IEnumerable<Usuario> BuscarPorTipo(string tipo, List<string> habilidades)
        {
            return _usuarioService.BuscarPorTipo(tipo, habilidades);
        }

        public Usuario GetByEmail(string email)
        {
            return _usuarioService.GetByEmail(email);
        }

        public Dictionary<string, string> GetDashboard(string tipo, DateTime inicio, DateTime fim)
        {
            return _usuarioService.GetDashboard(tipo, inicio, fim);
        }
    }
}
