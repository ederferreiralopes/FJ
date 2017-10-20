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
    public class VagaService : ServiceBase<Vaga>, IVagaService
    {
        private readonly IVagaRepository _vagaRepository;

        public VagaService(IVagaRepository vagaRepository)
            : base(vagaRepository)
        {
            _vagaRepository = vagaRepository;
        }

        public IEnumerable<Vaga> BuscarPorEmpresa(Guid id)
        {
            return _vagaRepository.GetAll().Where(v => v.EmpresaId == id);
        }

        public IEnumerable<Vaga> BuscarPaginadaPorEmpresa(Guid id, int pagina)
        {
            return _vagaRepository.GetAll().Where(v => v.EmpresaId == id).Skip((pagina - 1) * 10).Take(10);
        }

        public IEnumerable<Vaga> BuscarVagas(List<string> habilidades, int pagina)
        {
            var query = "{ 'Habilidades' : {  $in : [";
            foreach (var item in habilidades)
            {
                query += "'" + item + "',";
            }

            query = query + "]}}";

            return _vagaRepository.Find(query, pagina - 1);
        }
    }
}
