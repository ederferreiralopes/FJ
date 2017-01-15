using FinderJobs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Application.Interface
{
    public interface IUsuarioAppService : IAppServiceBase<Usuario>
    {
        IEnumerable<Usuario> BuscarPorTipo(string tipo);

        bool ValidarLogin(string login);

        Usuario ValidarAcesso(string login, string senha);

        Usuario GetByEmail(string email);
    }
}
