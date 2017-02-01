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

                    vaga.Descricao = model.Descricao;
                    var habilidadesExistentes = model.Habilidades.Where(x => !x.Id.Equals(x.Nome)).ToList();

                    // Grava Habilidades Novas
                    foreach (var item in model.Habilidades.Where(x => x.Id.Equals(x.Nome)))
                    {
                        var habilidadeNova = new Domain.Entities.Habilidade { Nome = item.Nome };
                        habilidadeNova.Id = (Guid)_habilidadeService.Insert(habilidadeNova);
                        habilidadesExistentes.Add(habilidadeNova);
                    }

                    vaga.Habilidades = habilidadesExistentes;

                    if (model.Id == new Guid())
                    {
                        vaga.EmpresaId = model.IdEmpresa;
                        vaga.Cep = model.Cep;
                        vaga.DataCadastro = DateTime.Now;
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

        public ActionResult PesquisaEmpresa(string id)
        {
            var empresaId = Guid.Parse(id);
            var vagas = _vagaService.BuscarPorEmpresa(empresaId).ToList();
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
                                                  Cep = vaga.Cep,
                                                  Descricao = vaga.Descricao,
                                                  Habilidades = vaga.Habilidades,
                                              }).ToList();
                var usuarios = _usuarioService.BuscarPorTipo(UsuarioTipo.Candidato.ToString()).ToList();

                empresaVagaViewModel.Candidatos = (from usuario in usuarios
                                                   select new ViewModels.CandidatoDistanciaViewModel
                                                   {
                                                       Id = usuario.Id,
                                                       Celular = usuario.Celular,
                                                       Nome = usuario.Nome,
                                                       Habilidades = JsonConvert.SerializeObject(usuario.Habilidades),
                                                       EnderecoCep = usuario.Endereco.Cep,
                                                       Email = usuario.Email,
                                                   }).ToList();

                empresaVagaViewModel = CalcularDistancia(empresaVagaViewModel);
                empresaVagaViewModel = CalcularAderencia(empresaVagaViewModel);
                empresaVagaViewModel = EscolherCandidatoVaga(empresaVagaViewModel);

                var data = new List<object>();

                empresaVagaViewModel.Vagas = empresaVagaViewModel.Vagas.OrderBy(x => x.DataCadastro).ToList();

                foreach (var x in empresaVagaViewModel.Vagas)
                {
                    data.Add(new
                    {
                        IdVaga = x.Id,
                        DataCadastro = x.DataCadastro.ToShortDateString() + " " + x.DataCadastro.ToShortTimeString(),
                        Descricao = x.Descricao,
                        Cep = x.Cep,
                        Habilidades = x.Habilidades,
                        IdCandidato = x.UsuarioId,
                        Candidato = x.UsuarioNome,
                        Distancia = !string.IsNullOrWhiteSpace(x.Distancia) ? x.Distancia + " " + x.Unidade : "",
                        Aderencia = !string.IsNullOrWhiteSpace(x.Porcentagem) ? x.Porcentagem + " %" : ""
                    });
                }

                return Json(new { data }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PesquisaCandidato(string id)
        {
            var usuarioId = Guid.Parse(id);
            var usuario = _usuarioService.GetById(usuarioId);
            var viewModel = new CandidatoVagaViewModel
            {
                Origem = new UsuarioViewModel
                {
                    Id = usuario.Id,
                    CpfCnpj = usuario.CpfCnpj,
                    Celular = usuario.Celular,
                    Nome = usuario.Nome,
                    Pago = usuario.Pago,
                    Tipo = usuario.Tipo,
                    Habilidades = (from h in usuario.Habilidades select new HabilidadeViewModel { Id = h.Id.ToString(), Nome = h.Nome}).ToList(),
                    Endereco = usuario.Endereco,
                    Email = usuario.Email,
                    DataCadastro = usuario.DataCadastro,
                    Anonimo = usuario.Anonimo,
                }
            };

            var vagas = _vagaService.GetAll().ToList();
            if (vagas != null && vagas.Count > 0)
            {
                var vagasViewModel = new List<VagaDistanciaViewModel>();
                foreach (var item in vagas)
                {
                    vagasViewModel.Add(
                        new VagaDistanciaViewModel
                        {
                            Id = item.Id,
                            IdEmpresa = item.EmpresaId,                            
                            Cep = item.Cep,
                            DataCadastro = item.DataCadastro,
                            Descricao = item.Descricao,                            
                            Habilidades = (from h in item.Habilidades select new HabilidadeViewModel { Id = h.Id.ToString(), Nome = h.Nome }).ToList(),
                        });
                }
                viewModel.Destinos = vagasViewModel;
                viewModel = CalcularDistancia(viewModel);
                viewModel = CalcularAderencia(viewModel);

                var data = new List<object>();

                viewModel.Destinos = viewModel.Destinos.OrderBy(x => x.DataCadastro).ToList();

                foreach (var x in viewModel.Destinos)
                {
                    data.Add(new
                    {
                        IdVaga = x.Id,
                        DataCadastro = x.DataCadastro.ToShortDateString() + " " + x.DataCadastro.ToShortTimeString(),
                        Descricao = x.Descricao,
                        Cep = x.Cep,
                        Habilidades = x.Habilidades,
                        IdEmpresa = x.IdEmpresa,
                        Empresa = x.Empresa,
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
                TempData["mensagem"] = "Ocorreu um erro!";
                throw erro;
            }
        }

        private EmpresaVagaViewModel CalcularDistancia(EmpresaVagaViewModel viewModel)
        {
            var url = "https://maps.googleapis.com/maps/api/distancematrix/json";
            var origens = string.Empty;
            foreach (var item in viewModel.Vagas)
            {
                origens += item.Cep + "|";
            }
            var destinos = string.Empty;
            foreach (var item in viewModel.Candidatos)
            {
                destinos += item.EnderecoCep + "|";
            }
            origens = origens.Substring(0, origens.Length - 1);
            destinos = destinos.Substring(0, destinos.Length - 1);

            var queryString = "?origins=" + origens + "&destinations=" + destinos + "&mode=driving" + "&key=AIzaSyCycG73VSX4N6sFxieKuCpYBnAKCJrG3XI";
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

                int indiceVaga = 0;
                foreach (var row in respostaJson.rows)
                {
                    var calculosVaga = new List<CalculosVaga>();
                    int indiceCandidadto = 0;
                    foreach (var element in row.elements)
                    {
                        calculosVaga.Add(new CalculosVaga { CandidatoId = viewModel.Candidatos[indiceCandidadto].Id, Distancia = element.distance.text.ToString() });
                        indiceCandidadto++;
                    }
                    viewModel.Vagas[indiceVaga].CalculosVaga = calculosVaga;
                    indiceVaga++;
                }

                return viewModel;
            }
            catch (Exception erro)
            {
                TempData["mensagem"] = "Ocorreu um erro!";
                throw erro;
            }
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
                        if (item.Habilidades.Exists(x => x.Nome.Contains(habilidade.Nome)))
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
                int indiceCandidato = 0;
                foreach (var candidato in viewModel.Candidatos)
                {
                    if (!string.IsNullOrEmpty(candidato.Habilidades))
                    {
                        var habilidadesCandidato = JsonConvert.DeserializeObject<List<HabilidadeViewModel>>(candidato.Habilidades);
                        decimal porcentagem = 0;
                        var taxa = 100 / vaga.Habilidades.Where(x => x != null).ToArray().Length;

                        foreach (var habilidade in vaga.Habilidades)
                        {
                            if (habilidadesCandidato.Exists(x => x.Nome.Contains(habilidade.Nome)))
                                porcentagem += taxa;
                        }

                        vaga.CalculosVaga[indiceCandidato].Aderencia = porcentagem;
                    }
                }
            }

            return viewModel;
        }

        private EmpresaVagaViewModel EscolherCandidatoVaga(EmpresaVagaViewModel viewModel)
        {
            var campeao = new CalculosVaga();
            foreach (var vaga in viewModel.Vagas)
            {
                foreach (var calculo in vaga.CalculosVaga)
                {
                    if (campeao.CandidatoId == new Guid())
                        campeao = calculo;
                    else if (calculo.Aderencia > campeao.Aderencia)
                        campeao = calculo;
                }

                vaga.UsuarioId = campeao.CandidatoId;
                vaga.UsuarioNome = viewModel.Candidatos.Where(x => x.Id == campeao.CandidatoId).FirstOrDefault().Nome;
                vaga.UsuarioCep = viewModel.Candidatos.Where(x => x.Id == campeao.CandidatoId).FirstOrDefault().EnderecoCep;
                vaga.Porcentagem = campeao.Aderencia.ToString();
                vaga.Distancia = campeao.Distancia;
            }

            return viewModel;
        }
    }
}
