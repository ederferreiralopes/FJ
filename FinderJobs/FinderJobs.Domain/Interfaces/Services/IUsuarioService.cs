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

        bool ValidarLogin(string login);

        Usuario ValidarAcesso(string login, string senha);

        Usuario GetByEmail(string email);
    }
}
