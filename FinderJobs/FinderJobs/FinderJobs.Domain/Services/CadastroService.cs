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
    public class CadastroService : ServiceBase<Cadastro>, ICadastroService
    {
        private readonly ICadastroRepository _usuarioRepository;

        public CadastroService(ICadastroRepository usuarioRepository)
            : base(usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public IEnumerable<Cadastro> BuscarPorTipo(string tipo)
        {
            return _usuarioRepository.GetAll().Where(u => u.Plano.Tipo.Equals(tipo));
        }

        public IEnumerable<Cadastro> BuscarPorTipo(string tipo, List<string> habilidades)
        {
            var query = "{ 'Tipo' : '" + tipo + "'}";
            if (habilidades != null && habilidades.Count > 0)
            {
                query = "{ 'Tipo' : '" + tipo + "', 'Habilidades' : {  $in : [";
                foreach (var item in habilidades)
                {
                    query += "'" + item + "',";
                }

                query = query + "]}}";
            }

            return _usuarioRepository.Find(query, 0);
        }

        public Cadastro GetByEmail(string email)
        {            
            return _usuarioRepository.SearchFor(u => u.Email == email).FirstOrDefault();
        }

        public Dictionary<string, string> GetDashboard(string tipo, DateTime ano)
        {
            return _usuarioRepository.GetDashboard(tipo, ano);
        }
    }
}
