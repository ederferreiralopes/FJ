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
            if (!string.IsNullOrWhiteSpace(model.Descricao) && model.Habilidades != null && model.Habilidades.Length > 0)
            {
                try
                {
                    var vaga = new Domain.Entities.Vaga();
                    if (model.Id > 0)
                        vaga = _vagaService.GetById(model.Id);

                    vaga.Descricao = model.Descricao;

                    var habilidades = JsonConvert.DeserializeObject<List<HabilidadeJson>>(model.Habilidades);
                    var habilidadesExistentes = habilidades.Where(x => !x.Id.Equals(x.Nome)).ToList();

                    // Grava Habilidades Novas
                    foreach (var item in habilidades.Where(x => x.Id.Equals(x.Nome)))
                    {
                        var id = (int)_habilidadeService.Add(new Domain.Entities.Habilidade { Nome = item.Nome });
                        habilidadesExistentes.Add(new HabilidadeJson { Id = id.ToString(), Nome = item.Nome });
                    }

                    vaga.Habilidades = JsonConvert.SerializeObject(habilidadesExistentes);

                    if (model.Id == 0)
                    {
                        vaga.Empresa = new Domain.Entities.Usuario { Id = model.IdEmpresa };
                        vaga.Cep = model.Cep;
                        vaga.DataCadastro = DateTime.Now;
                        vaga.Ativo = true;
                        model.Id = (int)_vagaService.Add(vaga);
                    }
                    else
                        _vagaService.Update(vaga);

                    return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { sucesso = false, mensagem = "Preencha todos os campos" }, JsonRequestBehavior.AllowGet);
        }     

        public ActionResult PesquisaEmpresa(int id)
        {
            var vagas = _vagaService.BuscarPorEmpresa(id).ToList();
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
                                                   Habilidades = usuario.Habilidades,
                                                   EnderecoCep = usuario.EnderecoCep,
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

        public ActionResult PesquisaCandidato(int id)
        {
            var usuario = _usuarioService.GetById(id);
            var viewModel = new CandidatoVagaViewModel
            {
                Origem = new UsuarioViewModel
                {
                    Id = usuario.Id,
                    CpfCnpj = usuario.CpfCnpj,
                    Celular = usuario.Celular,
                    Login = usuario.Login,
                    Nome = usuario.Nome,
                    Pago = usuario.Pago,
                    Senha = usuario.Senha,
                    Tipo = usuario.Tipo,
                    Habilidades = usuario.Habilidades,
                    EnderecoUF = usuario.EnderecoUF,
                    EnderecoNumero = usuario.EnderecoNumero,
                    EnderecoLogradouro = usuario.EnderecoLogradouro,
                    EnderecoCidade = usuario.EnderecoCidade,
                    EnderecoBairro = usuario.EnderecoBairro,
                    EnderecoCep = usuario.EnderecoCep,
                    Email = usuario.Email,
                    DataCadastro = usuario.DataCadastro,
                    Anonimo = usuario.Anonimo,
                }
            };

            var vagas = _vagaService.GetAll().ToList();
            if (vagas != null)
            {
                var vagasViewModel = new List<VagaDistanciaViewModel>();
                foreach (var item in vagas)
                {
                    vagasViewModel.Add(
                        new VagaDistanciaViewModel
                        {
                            Id = item.Id,
                            IdEmpresa = item.Empresa.Id,
                            Empresa = item.Empresa.Nome,
                            Cep = item.Cep,
                            DataCadastro = item.DataCadastro,
                            Descricao = item.Descricao,
                            Habilidades = item.Habilidades,
                        });
                }
                viewModel.Destinos = vagasViewModel;
                viewModel = CalcularDistancia(viewModel);
                viewModel = CalcularAderencia(viewModel);
            }

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

        private CandidatoVagaViewModel CalcularDistancia(CandidatoVagaViewModel viewModel)
        {
            var url = "https://maps.googleapis.com/maps/api/distancematrix/json";
            var destinos = string.Empty;
            foreach (var item in viewModel.Destinos)
            {
                destinos += item.Cep + "|";
            }
            destinos = destinos.Substring(0, destinos.Length - 1);

            var queryString = "?origins=" + viewModel.Origem.EnderecoCep + "&destinations=" + destinos + "&mode=driving" + "&key=AIzaSyCycG73VSX4N6sFxieKuCpYBnAKCJrG3XI";
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
            var HabilidadesOrigem = JsonConvert.DeserializeObject<List<HabilidadeJson>>(viewModel.Origem.Habilidades);

            foreach (var item in viewModel.Destinos)
            {
                if (!string.IsNullOrEmpty(item.Habilidades))
                {
                    var habilidadeJson = JsonConvert.DeserializeObject<List<HabilidadeJson>>(item.Habilidades);
                    decimal porcentagem = 0;
                    var indice = 100 / habilidadeJson.Where(x => x != null).ToArray().Length;

                    foreach (var habilidade in HabilidadesOrigem)
                    {
                        if (habilidadeJson.Exists(x => x.Nome.Contains(habilidade.Nome)))
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
                var HabilidadesVaga = JsonConvert.DeserializeObject<List<HabilidadeJson>>(vaga.Habilidades);
                int indiceCandidato = 0;
                foreach (var candidato in viewModel.Candidatos)
                {                    
                    if (!string.IsNullOrEmpty(candidato.Habilidades))
                    {
                        var habilidadesCandidato = JsonConvert.DeserializeObject<List<HabilidadeJson>>(candidato.Habilidades);
                        decimal porcentagem = 0;
                        var taxa = 100 / HabilidadesVaga.Where(x => x != null).ToArray().Length;

                        foreach (var habilidade in HabilidadesVaga)
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
                    if (campeao.CandidatoId == 0)
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
