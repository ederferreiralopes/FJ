using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Interfaces.Services
{
    public interface ICadastroService: IServiceBase<Cadastro>
    {
        IEnumerable<Cadastro> BuscarPorTipo(string tipo);

        IEnumerable<Cadastro> BuscarPorTipo(string tipo, List<string> habilidades);

        Cadastro GetByEmail(string email);

        Dictionary<string, string> GetDashboard(string tipo, DateTime ano);
    }
}
