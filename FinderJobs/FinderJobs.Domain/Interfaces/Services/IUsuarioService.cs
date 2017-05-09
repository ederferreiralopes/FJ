using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Domain.Interfaces.Services
{
    public interface IUsuarioService: IServiceBase<Usuario>
    {
        IEnumerable<Usuario> BuscarPorTipo(string tipo);

        IEnumerable<Usuario> BuscarPorTipo(string tipo, List<string> habilidades);

        Usuario GetByEmail(string email);

        Dictionary<string, string> GetDashboard(string tipo, DateTime inicio, DateTime fim);
    }
}
