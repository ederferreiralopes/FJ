using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Application.Interface
{
    public interface ICadastroAppService : IAppServiceBase<Cadastro>
    {
        IEnumerable<Cadastro> BuscarPorTipo(string tipo);

        IEnumerable<Cadastro> BuscarPorTipo(string tipo, List<string> habilidades);

        Cadastro GetByEmail(string email);

        Dictionary<string, string> GetDashboard(string tipo, DateTime ano);
    }
}
