using FinderJobs.Site.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using FinderJobs.Application.Interface;
using Newtonsoft.Json;
using FinderJobs.Domain.Entities;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FinderJobs.Site.Controllers
{
    [Authorize]
    public class VagasController : Controller
    {
        private readonly IUsuarioAppService _usuarioService;
        private readonly IHabilidadeAppService _habilidadeService;
        private readonly IVagaAppService _vagaService;

        public VagasController(IHabilidadeAppService habilidadeService, IVagaAppService vagaService, IUsuarioAppService usuarioService)
        {
            _habilidadeService = habilidadeService;
            _vagaService = vagaService;
            _usuarioService = usuarioService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CadastrarVaga(VagaViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Descricao) && model.Habilidades != null && model.Habilidades.Count > 0)
            {
                try
                {
                    var vaga = new Domain.Entities.Vaga();
                    if (model.Id != new Guid())
                        vaga = _vagaService.GetById(model.Id);

                    var habilidadesModel = (from h in model.Habilidades where h.Id.Equals(h.Nome) select h.Nome).ToList();

                    foreach (var item in habilidadesModel)
                    {
                        var hab = _habilidadeService.BuscarPorNome(item);
                        if (hab == null || hab.Count() == 0)
                            _habilidadeService.Insert(new Habilidade { Nome = item });
                    }

                    vaga.Habilidades = (from h in model.Habilidades select h.Nome).ToList();
                    vaga.Descricao = model.Descricao;

                    if (model.Id == new Guid())
                    {
                        vaga.EmpresaId = model.EmpresaId;
                        vaga.EmpresaNome = model.EmpresaNome;
                        vaga.EmpresaUrlAvatar = model.EmpresaUrlAvatar;
                        vaga.Cep = model.Cep;
                        vaga.DataCadastro = DateTime.Now;
                        vaga.DataExpiracao = DateTime.Now.AddDays(10);
                        vaga.Ativo = true;
                        var resultado = _vagaService.Insert(vaga);
                    }
                    else
                    {
                        vaga.DataAlteracao = DateTime.Now;
                        _vagaService.Update(vaga);
                    }

                    return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { sucesso = false, mensagem = "Preencha todos os campos" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Carregar()
        {
            var cadastro = _usuarioService.GetByEmail(User.Identity.Name);            
            var vagas = _vagaService.BuscarPorEmpresa(cadastro.Id).ToList();

            if (vagas != null && vagas.Count > 0)
            {
                var dados = new List<object>();

                vagas = vagas.OrderBy(x => x.DataCadastro).ToList();

                foreach (var x in vagas)
                {
                    dados.Add(new
                    {
                        IdVaga = x.Id,                        
                        DataCadastro = x.DataCadastro.ToShortDateString(),
                        DataExpiracao = x.DataExpiracao.ToShortDateString(),
                        Descricao = x.Descricao,
                        Cep = x.Cep,
                        Habilidades = x.Habilidades,
                        Ativo = x.Ativo
                    });
                }

                return Json(new { dados }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PesquisaEmpresa(string id, int pagina = 1)
        {
            var empresaId = Guid.Parse(id);
            var vagas = _vagaService.BuscarPaginadaPorEmpresa(empresaId, pagina).ToList();
            if (vagas != null && vagas.Count > 0)
            {
                var empresaVagaViewModel = new EmpresaVagaViewModel
                {
                    Vagas = new List<EmpresaDistanciaViewModel>(),
                    Candidatos = new List<CandidatoDistanciaViewModel>()
                };

                empresaVagaViewModel.Vagas = (from vaga in vagas
                                              select new EmpresaDistanciaViewModel
                                              {
                                                  Id = vaga.Id,
                                                  DataCadastro = vaga.DataCadastro,
                                                  DataExpiracao = vaga.DataExpiracao,
                                                  Cep = vaga.Cep,
                                                  Descricao = vaga.Descricao,
                                                  Habilidades = vaga.Habilidades,
                                              }).ToList();

                var vagasHabilidades = new List<string>();
                foreach (var vaga in vagas)
                {
                    foreach (var habilidade in vaga.Habilidades)
                    {
                        if (!vagasHabilidades.Exists(x => x == habilidade))
                            vagasHabilidades.Add(habilidade);
                    }

                }

                var candidatos = _usuarioService.BuscarPorTipo(UsuarioTipo.Candidato.ToString(), vagasHabilidades).ToList();

                empresaVagaViewModel.Candidatos = (from usuario in candidatos
                                                   select new ViewModels.CandidatoDistanciaViewModel
                                                   {
                                                       Id = usuario.Id,
                                                       Nome = usuario.Anonimo ? "" : usuario.Nome,
                                                       Habilidades = JsonConvert.SerializeObject(usuario.Habilidades),
                                                       EnderecoCep = usuario.Endereco.Cep,
                                                       UrlAvatar = usuario.Anonimo ? "" : usuario.UrlAvatar,
                                                   }).ToList();

                empresaVagaViewModel = CalcularAderencia(empresaVagaViewModel);

                empresaVagaViewModel = CalcularDistancia(empresaVagaViewModel);

                empresaVagaViewModel = EscolherCandidatoVaga(empresaVagaViewModel);

                var data = new List<object>();

                empresaVagaViewModel.Vagas = empresaVagaViewModel.Vagas.OrderBy(x => x.DataCadastro).ToList();

                foreach (var x in empresaVagaViewModel.Vagas)
                {
                    data.Add(new
                    {
                        IdVaga = x.Id,
                        CandidatoId = x.CandidatoId,
                        CandidatoNome = x.CandidatoNome,
                        CandidatoUrlAvatar = x.CandidatoUrlAvatar,
                        DataCadastro = x.DataCadastro.ToShortDateString(),
                        DataExpiracao = x.DataExpiracao.ToShortDateString(),
                        Descricao = x.Descricao,
                        Cep = x.Cep,
                        Habilidades = x.Habilidades,
                        Distancia = !string.IsNullOrWhiteSpace(x.Distancia) ? x.Distancia + " " + x.Unidade : "",
                        Aderencia = !string.IsNullOrWhiteSpace(x.Porcentagem) ? x.Porcentagem + " %" : ""
                    });
                }

                return Json(new { data }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PesquisaCandidato(string id, int pagina = 1)
        {
            var usuarioId = Guid.Parse(id);
            var usuario = _usuarioService.GetById(usuarioId);
            var viewModel = new CandidatoVagaViewModel
            {
                Origem = new UsuarioViewModel
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Habilidades = (from h in usuario.Habilidades select new HabilidadeViewModel { Id = "", Nome = h }).ToList(),
                    Endereco = usuario.Endereco,
                }
            };

            var vagas = _vagaService.BuscarVagas(usuario.Habilidades, pagina).ToList();
            if (vagas != null && vagas.Count > 0)
            {
                var vagasViewModel = new List<VagaDistanciaViewModel>();
                foreach (var item in vagas)
                {
                    vagasViewModel.Add(
                        new VagaDistanciaViewModel
                        {
                            Id = item.Id,
                            EmpresaId = item.EmpresaId,
                            EmpresaNome = item.EmpresaNome,
                            EmpresaUrlAvatar = item.EmpresaUrlAvatar,
                            Cep = item.Cep,
                            DataCadastro = item.DataCadastro,
                            DataExpiracao = item.DataExpiracao,
                            Descricao = item.Descricao,
                            Habilidades = (from h in item.Habilidades select h).ToList(),
                        });
                }

                viewModel.Destinos = vagasViewModel;

                viewModel = CalcularAderencia(viewModel);

                viewModel = CalcularDistancia(viewModel);

                var data = new List<object>();

                viewModel.Destinos = viewModel.Destinos.OrderBy(x => x.DataCadastro).ToList();

                foreach (var x in viewModel.Destinos)
                {
                    data.Add(new
                    {
                        IdVaga = x.Id,
                        EmpresaId = x.EmpresaId,
                        EmpresaNome = x.EmpresaNome,
                        EmpresaUrlAvatar = x.EmpresaUrlAvatar,
                        DataCadastro = x.DataCadastro.ToShortDateString(),
                        DataExpiracao = x.DataExpiracao.ToShortDateString(),
                        Descricao = x.Descricao,
                        Cep = x.Cep,
                        Habilidades = x.Habilidades,
                        Distancia = !string.IsNullOrWhiteSpace(x.Distancia) ? x.Distancia + " " + x.Unidade : "",
                        Aderencia = !string.IsNullOrWhiteSpace(x.Porcentagem) ? x.Porcentagem + " %" : ""
                    });
                }

                return Json(new { data }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

        private CandidatoVagaViewModel CalcularDistancia(CandidatoVagaViewModel viewModel)
        {
            var url = "https://maps.googleapis.com/maps/api/distancematrix/json";
            var destinos = string.Empty;
            foreach (var item in viewModel.Destinos)
            {
                destinos += item.Cep + "|";
            }
            destinos = destinos.Substring(0, destinos.Length - 1);

            var queryString = "?origins=" + viewModel.Origem.Endereco.Cep + "&destinations=" + destinos + "&mode=driving" + "&key=AIzaSyCycG73VSX4N6sFxieKuCpYBnAKCJrG3XI";
            try
            {
                var distancia = string.Empty;
                RespostaViewDistancia respostaJson = null;
                var request = (HttpWebRequest)WebRequest.Create(url + queryString);
                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    distancia = reader.ReadToEnd();
                    respostaJson = JsonConvert.DeserializeObject<RespostaViewDistancia>(distancia);
                }

                var indece = 0;
                foreach (var item in respostaJson.rows[0].elements)
                {
                    viewModel.Destinos[indece].Distancia = item.distance.text.Replace(" m", "").Replace(" km", "");
                    viewModel.Destinos[indece].Unidade = item.distance.text.Contains(" m") ? "m" : "km";
                    indece++;
                }

                return viewModel;
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        private EmpresaVagaViewModel CalcularDistancia(EmpresaVagaViewModel viewModel)
        {
            if (viewModel.Vagas.Count > 0 && viewModel.Candidatos.Count > 0)
            {
                try
                {
                    foreach (var vaga in viewModel.Vagas)
                    {
                        var url = "https://maps.googleapis.com/maps/api/distancematrix/json";
                        var origem = string.Empty;
                        var destinos = string.Empty;

                        origem += vaga.Cep;

                        foreach (var calc in vaga.CalculosVaga)
                        {
                            destinos += calc.CandidatoCep + "|";
                        }

                        destinos = destinos.Substring(0, destinos.Length - 1);

                        var queryString = "?origins=" + origem + "&destinations=" + destinos + "&mode=driving" + "&key=AIzaSyCycG73VSX4N6sFxieKuCpYBnAKCJrG3XI";

                        var distancia = string.Empty;
                        RespostaViewDistancia respostaJson = null;
                        var request = (HttpWebRequest)WebRequest.Create(url + queryString);
                        var response = request.GetResponse();

                        using (var stream = response.GetResponseStream())
                        {
                            var reader = new StreamReader(stream, Encoding.UTF8);
                            distancia = reader.ReadToEnd();
                            respostaJson = JsonConvert.DeserializeObject<RespostaViewDistancia>(distancia);
                        }

                        foreach (var row in respostaJson.rows)
                        {
                            int indiceCandidadto = 0;
                            foreach (var element in row.elements)
                            {
                                vaga.CalculosVaga.Where(x => x.CandidatoId == vaga.CalculosVaga[indiceCandidadto].CandidatoId).FirstOrDefault().Distancia = element.distance.text.ToString();
                                indiceCandidadto++;
                            }

                        }
                    }
                }
                catch (Exception erro)
                {
                    throw erro;
                }
            }

            return viewModel;
        }

        private CandidatoVagaViewModel CalcularAderencia(CandidatoVagaViewModel viewModel)
        {
            foreach (var item in viewModel.Destinos)
            {
                if (item.Habilidades != null && item.Habilidades.Count > 0)
                {
                    decimal porcentagem = 0;
                    var indice = 100 / item.Habilidades.Where(x => x != null).ToArray().Length;

                    foreach (var habilidade in viewModel.Origem.Habilidades)
                    {
                        if (item.Habilidades.Exists(x => x.Contains(habilidade.Nome)))
                            porcentagem += indice;
                    }

                    item.Porcentagem = porcentagem.ToString();
                }
            }

            return viewModel;
        }

        private EmpresaVagaViewModel CalcularAderencia(EmpresaVagaViewModel viewModel)
        {
            foreach (var vaga in viewModel.Vagas)
            {
                if (vaga.CalculosVaga == null)
                    vaga.CalculosVaga = new List<CalculosVaga>();

                foreach (var candidato in viewModel.Candidatos)
                {
                    if (!string.IsNullOrEmpty(candidato.Habilidades))
                    {
                        var habilidadesCandidato = JsonConvert.DeserializeObject<List<string>>(candidato.Habilidades);
                        decimal porcentagem = 0;
                        var taxa = 100 / vaga.Habilidades.Where(x => x != null).ToArray().Length;

                        foreach (var habilidade in vaga.Habilidades)
                        {
                            if (habilidadesCandidato.Exists(x => x.Contains(habilidade)))
                                porcentagem += taxa;
                        }

                        vaga.CalculosVaga.Add(new CalculosVaga { CandidatoId = candidato.Id, Aderencia = porcentagem, CandidatoCep = candidato.EnderecoCep, UrlAvatar = candidato.UrlAvatar });
                    }
                }

                vaga.CalculosVaga = vaga.CalculosVaga.OrderBy(x => x.Aderencia).Take(100).ToList();
            }

            return viewModel;
        }

        private EmpresaVagaViewModel EscolherCandidatoVaga(EmpresaVagaViewModel viewModel)
        {
            foreach (var vaga in viewModel.Vagas)
            {
                var campeao = new CalculosVaga();
                foreach (var calculo in vaga.CalculosVaga)
                {
                    if (campeao.CandidatoId == new Guid())
                        campeao = calculo;
                    else if (calculo.Aderencia > campeao.Aderencia)
                        campeao = calculo;
                }

                //TO DO
                // escolher candidato mais perto
                if (campeao.Aderencia > 0)
                {
                    vaga.CandidatoId = campeao.CandidatoId;
                    vaga.CandidatoNome = viewModel.Candidatos.Where(x => x.Id == campeao.CandidatoId).FirstOrDefault() != null ? viewModel.Candidatos.Where(x => x.Id == campeao.CandidatoId).FirstOrDefault().Nome : "";
                    vaga.CandidatoCep = viewModel.Candidatos.Where(x => x.Id == campeao.CandidatoId).FirstOrDefault() != null ? viewModel.Candidatos.Where(x => x.Id == campeao.CandidatoId).FirstOrDefault().EnderecoCep : "";
                    vaga.Porcentagem = campeao.Aderencia.ToString();
                    vaga.Distancia = campeao.Distancia;
                    vaga.CandidatoUrlAvatar = campeao.UrlAvatar;
                }
            }

            return viewModel;
        }
    }
}
