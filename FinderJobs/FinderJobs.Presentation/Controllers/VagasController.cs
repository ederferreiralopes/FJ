using FinderJobs.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using FinderJobs.Application.Interface;
using Newtonsoft.Json;
using FinderJobs.Domain.Entities;
using AutoMapper;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FinderJobs.MVC.Controllers
{
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

        public ActionResult PesquisaEmpresa(int id = 0)
        {
            var listaDeVagas = new EmpresaViewModel
            {
                Vagas = new List<VagaViewModel>()
            };

            var vagas = _vagaService.BuscarPorEmpresa(id).ToList();
            if (vagas != null)
            {
                var vagasViewModel = new List<VagaViewModel>();
                foreach (var item in vagas)
                {
                    vagasViewModel.Add(
                        new VagaViewModel
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

                var usuarios = _usuarioService.BuscarPorTipo(UsuarioTipo.Candidato.ToString()).ToList();
                var usuariosViewModel = Mapper.Map<List<Domain.Entities.Usuario>, List<UsuarioViewModel>>(usuarios);

                //Para cada vaga será procurado o candidato mais próximo com maior aderencia
                foreach (var vaga in vagasViewModel)
                {
                    var listaDeVagasTemp = new List<VagaViewModel>();
                    var vagaView = new VagaViewModel();
                    vagaView.Vaga = new ViewModels.Vaga
                    {
                        Id = vaga.Id,
                        Cep = vaga.Cep,
                        DataCadastro = vaga.DataCadastro,
                        Descricao = vaga.Descricao,
                        Empresa = vaga.Empresa,
                        Habilidades = vaga.Habilidades,
                        IdEmpresa = vaga.IdEmpresa
                    };

                    //colocar essa validacao de CEP na View
                    if (!string.IsNullOrWhiteSpace(vagaView.Vaga.Cep) && !vagaView.Vaga.Cep.Contains('-'))
                        vagaView.Vaga.Cep = vagaView.Vaga.Cep.Insert(5, "-");

                    if (usuarios.Count == 0)
                        listaDeVagas.Vagas.Add(vagaView);

                    else
                    {
                        VagaViewModel vagaViewAtual = null;

                        foreach (var usuario in usuariosViewModel)
                        {
                            vagaView.Candidato = usuario;

                            //colocar essa validacao de CEP na View
                            usuario.EnderecoCep = string.IsNullOrWhiteSpace(usuario.EnderecoCep) || usuario.EnderecoCep.Length > 7 ? usuario.EnderecoCep : usuario.EnderecoCep.Insert(5, "-");

                            listaDeVagasTemp.Add(PesquisarDistancia(vagaView));

                            foreach (var item in listaDeVagasTemp)
                            {
                                if (vagaViewAtual == null)
                                    vagaViewAtual = item;
                                else
                                {
                                    //Mesma unidade de medida da distancia                            
                                    if (vagaViewAtual.Pesquisa != null && vagaViewAtual.Pesquisa.Unidade != null && item.Pesquisa.Unidade.Equals(vagaViewAtual.Pesquisa.Unidade))
                                    {
                                        if (int.Parse(vagaViewAtual.Pesquisa.Distancia) > int.Parse(item.Pesquisa.Distancia))
                                            vagaViewAtual = item;
                                    }

                                    //Unidades de medida diferentes
                                    else if (item.Pesquisa != null && item.Pesquisa.Unidade != null && item.Pesquisa.Unidade.Equals("m"))
                                        vagaViewAtual = item;
                                }
                            }
                        }
                        listaDeVagas.Vagas.Add(vagaViewAtual);
                    }
                }
            }

            var data = new List<object>();

            listaDeVagas.Vagas = listaDeVagas.Vagas.OrderBy(x => x.Vaga.DataCadastro).ToList();

            foreach (var x in listaDeVagas.Vagas)
            {
                if (x.Candidato != null)
                {
                    data.Add(new
                    {
                        IdVaga = x.Vaga.Id,
                        DataCadastro = x.Vaga.DataCadastro.ToShortDateString() + " " + x.Vaga.DataCadastro.ToShortTimeString(),
                        Descricao = x.Vaga.Descricao,
                        Cep = x.Vaga.Cep,
                        Habilidades = x.Vaga.Habilidades,
                        IdCandidato = x.Candidato.Id,
                        Candidato = x.Candidato.Nome,
                        Distancia = x.Pesquisa != null ? x.Pesquisa.Distancia + " " + x.Pesquisa.Unidade : "",
                        Aderencia = x.Pesquisa != null ? x.Pesquisa.Porcentagem + " %" : ""
                    });
                }
            }

            var resultado = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    data
                }
            };

            return resultado;
        }

        public ActionResult PesquisaCandidato(int id)
        {
            var listaDeVagas = new EmpresaViewModel
            {
                Vagas = new List<VagaViewModel>()
            };

            var vagas = _vagaService.GetAll().ToList();
            if (vagas != null)
            {
                var vagasViewModel = new List<VagaViewModel>();
                foreach (var item in vagas)
                {
                    vagasViewModel.Add(
                        new VagaViewModel
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

                var usuario = _usuarioService.GetById(id);
                var usuariosViewModel = Mapper.Map<Domain.Entities.Usuario, UsuarioViewModel>(usuario);

                //Para cada vaga será procurado o candidato mais próximo com maior aderencia
                foreach (var vaga in vagasViewModel)
                {
                    var listaDeVagasTemp = new List<VagaViewModel>();
                    var vagaView = new VagaViewModel();
                    vagaView.Vaga = new ViewModels.Vaga
                    {
                        Id = vaga.Id,
                        Cep = vaga.Cep,
                        DataCadastro = vaga.DataCadastro,
                        Descricao = vaga.Descricao,
                        Empresa = vaga.Empresa,
                        Habilidades = vaga.Habilidades,
                        IdEmpresa = vaga.IdEmpresa
                    };

                    //colocar essa validacao de CEP na View
                    if (!string.IsNullOrWhiteSpace(vagaView.Vaga.Cep) && !vagaView.Vaga.Cep.Contains('-'))
                        vagaView.Vaga.Cep = vagaView.Vaga.Cep.Insert(5, "-");

                    if (usuario == null)
                        listaDeVagas.Vagas.Add(vagaView);

                    else
                    {
                        VagaViewModel vagaViewAtual = null;

                        vagaView.Candidato = usuariosViewModel;

                        //colocar essa validacao de CEP na View
                        usuario.EnderecoCep = string.IsNullOrWhiteSpace(usuario.EnderecoCep) || usuario.EnderecoCep.Length > 7 ? usuario.EnderecoCep : usuario.EnderecoCep.Insert(5, "-");

                        listaDeVagasTemp.Add(PesquisarDistancia(vagaView));

                        foreach (var item in listaDeVagasTemp)
                        {
                            if (vagaViewAtual == null)
                                vagaViewAtual = item;
                            else
                            {
                                //Mesma unidade de medida da distancia                            
                                if (vagaViewAtual.Pesquisa != null && vagaViewAtual.Pesquisa.Unidade != null && item.Pesquisa.Unidade.Equals(vagaViewAtual.Pesquisa.Unidade))
                                {
                                    if (int.Parse(vagaViewAtual.Pesquisa.Distancia) > int.Parse(item.Pesquisa.Distancia))
                                        vagaViewAtual = item;
                                }

                                //Unidades de medida diferentes
                                else if (item.Pesquisa != null && item.Pesquisa.Unidade != null && item.Pesquisa.Unidade.Equals("m"))
                                    vagaViewAtual = item;
                            }
                        }

                        listaDeVagas.Vagas.Add(vagaViewAtual);
                    }
                }
            }

            var data = new List<object>();

            listaDeVagas.Vagas = listaDeVagas.Vagas.OrderBy(x => x.Vaga.DataCadastro).ToList();

            foreach (var x in listaDeVagas.Vagas)
            {
                if (x.Candidato != null)
                {
                    data.Add(new
                    {
                        IdVaga = x.Vaga.Id,
                        DataCadastro = x.Vaga.DataCadastro.ToShortDateString() + " " + x.Vaga.DataCadastro.ToShortTimeString(),
                        Descricao = x.Vaga.Descricao,
                        Cep = x.Vaga.Cep,
                        Habilidades = x.Vaga.Habilidades,
                        IdEmpresa = x.Vaga.IdEmpresa,
                        Empresa = x.Vaga.Empresa,
                        Distancia = x.Pesquisa != null ? x.Pesquisa.Distancia + " " + x.Pesquisa.Unidade : "",
                        Aderencia = x.Pesquisa != null ? x.Pesquisa.Porcentagem + " %" : ""
                    });
                }
            }

            var resultado = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    data
                }
            };

            return resultado;
        }

        private VagaViewModel PesquisarDistancia(VagaViewModel vagaView)
        {
            var HabilidadesCandidato = JsonConvert.DeserializeObject<List<HabilidadeJson>>(vagaView.Candidato.Habilidades);

            var HabilidadesVaga = new string[50];

            if (!string.IsNullOrEmpty(vagaView.Vaga.Habilidades))
            {
                var habilidadeJson = JsonConvert.DeserializeObject<List<HabilidadeJson>>(vagaView.Vaga.Habilidades);
                int contador = 0;
                foreach (var item in habilidadeJson)
                {
                    HabilidadesVaga[contador] = item.Nome;
                    contador++;
                }
            }

            var url = "https://maps.googleapis.com/maps/api/distancematrix/json";
            var queryString = "?origins=" + vagaView.Candidato.EnderecoCep + "&destinations=" + vagaView.Vaga.Cep + "&mode=driving" + "&key=AIzaSyCycG73VSX4N6sFxieKuCpYBnAKCJrG3XI";
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

                distancia = respostaJson.rows.Count > 0 && respostaJson.rows[0].elements[0].distance != null ? respostaJson.rows[0].elements[0].distance.text : "";
                decimal porcentagem = 0, indice = 100 / HabilidadesVaga.Where(x => x != null).ToArray().Length;

                if (distancia.Contains(" m"))
                {
                    if (vagaView.Candidato != null && vagaView.Candidato.Habilidades != null)
                    {
                        //HabilidadesCandidato = vagaView.Candidato.Habilidades.Split(',');

                        foreach (var habilidade in HabilidadesCandidato)
                        {
                            if (HabilidadesVaga.Contains(habilidade.Nome))
                            {
                                porcentagem += indice;
                            }
                        }

                        vagaView.Pesquisa = new DistanciaViewModel { Porcentagem = porcentagem.ToString() };
                    }
                    else
                    {
                        vagaView.Pesquisa = new DistanciaViewModel { Porcentagem = "0" };
                    }

                    var pattern = "\\D";
                    var replacement = "";
                    distancia = Regex.Replace(distancia, pattern, replacement);

                    vagaView.Pesquisa.Distancia = distancia;
                    vagaView.Pesquisa.Unidade = "m";
                }

                if (distancia.Contains(" km"))
                {
                    if (vagaView.Candidato != null && vagaView.Candidato.Habilidades != null)
                    {
                        //HabilidadesCandidato = vagaView.Candidato.Habilidades.Split(',');
                        foreach (var habilidade in HabilidadesCandidato)
                        {
                            if (HabilidadesVaga.Contains(habilidade.Nome))
                            {
                                porcentagem += indice;
                            }
                        }

                        if (vagaView.Pesquisa == null)
                            vagaView.Pesquisa = new DistanciaViewModel();
                        vagaView.Pesquisa.Porcentagem = porcentagem.ToString();
                    }
                    else
                    {
                        if (vagaView.Pesquisa == null)
                            vagaView.Pesquisa = new DistanciaViewModel();
                        vagaView.Pesquisa.Porcentagem = "0";
                    }

                    if (distancia.Contains("."))
                    {
                        distancia = distancia.Substring(0, distancia.IndexOf("."));
                    }
                    var pattern = "\\D";
                    var replacement = "";
                    distancia = Regex.Replace(distancia, pattern, replacement);

                    vagaView.Pesquisa.Distancia = distancia;
                    vagaView.Pesquisa.Unidade = "km";
                }

                return vagaView;
            }
            catch (Exception erro)
            {
                TempData["mensagem"] = "Ocorreu um erro!";
                throw erro;
            }
        }

        public JsonResult ListarVagasDistanciaAderencia()
        {
            var idVaga = Request.QueryString[0];

            var vagas = _vagaService.GetAll().ToList();
            var vagasViewModel = Mapper.Map<List<Domain.Entities.Vaga>, List<VagaViewModel>>(vagas);

            var usuarios = _usuarioService.BuscarPorTipo(UsuarioTipo.Candidato.ToString()).ToList();
            var usuarioViewModel = Mapper.Map<List<Domain.Entities.Usuario>, List<UsuarioViewModel>>(usuarios);

            var listaDeVagas = new List<VagaViewModel>();

            // Para cada vaga será procurado o candidato mais próximo com maior aderencia
            foreach (var vaga in vagas)
            {
                var listaDeVagasTemp = new List<VagaViewModel>();
                var vagaView = new VagaViewModel();
                vagaView.Vaga = new ViewModels.Vaga
                {
                    Id = vaga.Id,
                    Cep = vaga.Cep,
                    DataCadastro = vaga.DataCadastro,
                    Descricao = vaga.Descricao,
                    Empresa = vaga.Empresa.Nome,
                    Habilidades = vaga.Habilidades,
                    IdEmpresa = vaga.Empresa.Id
                };

                // colocar essa validacao de CEP na View
                if (!vagaView.Vaga.Cep.Contains('-'))
                {
                    vagaView.Vaga.Cep = vagaView.Vaga.Cep.Insert(5, "-");
                }

                foreach (var usuario in usuarioViewModel)
                {
                    vagaView.Candidato = usuario;

                    // colocar essa validacao de CEP na View
                    if (!usuario.EnderecoCep.Contains('-'))
                    {
                        usuario.EnderecoCep = usuario.EnderecoCep.Insert(5, "-");
                    }

                    //???
                    //vagaView.Candidato = db.Usuarios.Where(usu => usu.Id == usuario.Id).ToList().FirstOrDefault();

                    listaDeVagasTemp.Add(PesquisarDistancia(vagaView));

                    VagaViewModel vagaViewAtual = null;

                    foreach (var item in listaDeVagasTemp)
                    {
                        if (vagaViewAtual == null)
                        {
                            vagaViewAtual = item;
                        }
                        else
                        {
                            // Mesma unidade de medida da distancia                            
                            if (item.Pesquisa.Unidade.Equals(vagaViewAtual.Pesquisa.Unidade))
                            {
                                if (int.Parse(vagaViewAtual.Pesquisa.Unidade) > int.Parse(item.Pesquisa.Unidade))
                                {
                                    vagaViewAtual = item;
                                }
                            }

                            // Unidades de medida diferentes
                            else if (item.Pesquisa.Unidade.Equals("m"))
                            {
                                vagaViewAtual = item;
                            }
                        }
                    }

                    listaDeVagas.Add(vagaViewAtual);
                }
            }

            // alterar para retornar uma lista de vagas

            var resultado = new List<object>();

            foreach (var item in listaDeVagas)
            {
                resultado.Add(new
                {
                    DataCadastro = item.Vaga.DataCadastro,
                    Empresa = item.Vaga.Empresa,
                    Descricao = item.Vaga.Descricao,
                    Cep = item.Vaga.Cep,
                    Habilidades = item.Vaga.Habilidades,
                    Candidato = item.Candidato.Nome,
                    Distancia = item.Pesquisa.Distancia + " " + item.Pesquisa.Unidade,
                    Porcentagem = item.Pesquisa.Porcentagem,
                    EmailCandidato = item.Candidato.Email
                });
            }

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}
