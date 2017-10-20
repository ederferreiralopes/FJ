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
    public class CadastroAppService : AppServiceBase<Cadastro>, ICadastroAppService
    {
        private readonly ICadastroService _usuarioService;

        public CadastroAppService(ICadastroService usuarioService)
            : base(usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IEnumerable<Cadastro> BuscarPorTipo(string tipo)
        {
            return _usuarioService.BuscarPorTipo(tipo);
        }

        public IEnumerable<Cadastro> BuscarPorTipo(string tipo, List<string> habilidades)
        {
            return _usuarioService.BuscarPorTipo(tipo, habilidades);
        }

        public Cadastro GetByEmail(string email)
        {
            return _usuarioService.GetByEmail(email);
        }

        public Dictionary<string, string> GetDashboard(string tipo, DateTime ano)
        {
            return _usuarioService.GetDashboard(tipo, ano);
        }
    }
}
