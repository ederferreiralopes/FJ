
using System;
using System.IO;
using System.Web.Mvc;
using FinderJobs.MVC.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using FinderJobs.Infra.Data;
using AutoMapper;

namespace FinderJobs.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly UsuarioRepository _usuarioRepository = new UsuarioRepository();
        private readonly VagaRepository _vagaRepository = new VagaRepository();
        public ActionResult IndexEmpresa(int? id)
        {
            if (TempData["Usuario"] != null)
            {
                ViewBag.TipoUsuario = "Empresa";
                return View("IndexEmpresa");
            }
            else
            {
                return View("../Acesso/Index");
            }
        }

        public ViewResult IndexCandidato(int? id)
        {
            if (TempData["Usuario"] != null)
            {
                ViewBag.TipoUsuario = "Candidato";
                return View("IndexCandidato");
            }
            else
            {
                return View("../Acesso/Index");
            }
        }

        public ActionResult PesquisaEmpresaJson(int id)
        {
            var listaDeVagas = new EmpresaViewModel();
            listaDeVagas.Vagas = new List<VagaViewModel>();

            var vagas = _vagaRepository.BuscarPorEmpresa(id);
            if (vagas != null)
            {
                var vagasViewModel = Mapper.Map<List<Domain.Entities.Vaga>, List<VagaViewModel>>(vagas);

                var usuarios = _usuarioRepository.BuscarPorTipo("Candidato");
                var usuarioViewModel = Mapper.Map<List<Domain.Entities.Usuario>, List<UsuarioViewModel>>(usuarios);



                //Para cada vaga será procurado o candidato mais próximo com maior aderencia
                foreach (var vaga in vagasViewModel)
                {
                    var listaDeVagasTemp = new List<VagaViewModel>();
                    var vagaView = new VagaViewModel();
                    vagaView.Vaga = new Vaga
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
                    {
                        vagaView.Vaga.Cep = vagaView.Vaga.Cep.Insert(5, "-");
                    }

                    if (usuarios.Count == 0)
                    {
                        listaDeVagas.Vagas.Add(vagaView);
                    }

                    else
                    {
                        VagaViewModel vagaViewAtual = null;

                        foreach (var usuario in usuarioViewModel)
                        {
                            vagaView.Candidato = usuario;

                            //colocar essa validacao de CEP na View
                            if (usuario.Cep != null && !usuario.Cep.Contains('-'))
                            {
                                usuario.Cep = usuario.Cep.Insert(5, "-");
                            }

                            //???
                            //vagaView.Candidato = db.Usuarios.Where(usu => usu.Id == usuario.Id).ToList().FirstOrDefault();

                            listaDeVagasTemp.Add(PesquisarDistancia(vagaView));

                            foreach (var item in listaDeVagasTemp)
                            {
                                if (vagaViewAtual == null)
                                {
                                    vagaViewAtual = item;
                                }
                                else
                                {
                                    //Mesma unidade de medida da distancia                            
                                    if (vagaViewAtual.Pesquisa != null && vagaViewAtual.Pesquisa.Unidade != null && item.Pesquisa.Unidade.Equals(vagaViewAtual.Pesquisa.Unidade))
                                    {
                                        if (int.Parse(vagaViewAtual.Pesquisa.Distancia) > int.Parse(item.Pesquisa.Distancia))
                                        {
                                            vagaViewAtual = item;
                                        }
                                    }

                                    //Unidades de medida diferentes
                                    else if (item.Pesquisa != null && item.Pesquisa.Unidade != null && item.Pesquisa.Unidade.Equals("m"))
                                    {
                                        vagaViewAtual = item;
                                    }
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

        [HttpPost]
        public ActionResult EditarUsuario(UsuarioViewModel model)
        {
            try
            {
                //using (var db = new FinderJobsContext())
                //{
                //    var atualiza = db.Usuarios.FirstOrDefault(cand => cand.Id == model.Id);
                //    if (atualiza != null)
                //    {
                //        atualiza.Nome = model.Nome;
                //        atualiza.Email = model.Email;
                //        atualiza.RgCnpj = model.RgCnpj;
                //        atualiza.Cep = model.Cep;
                //        atualiza.Pago = model.Pago;
                //        atualiza.Anonimo = model.Anonimo;
                //        atualiza.Habilidades = model.Habilidades;

                //        db.SaveChanges();
                //    }
                //}

                return Json(new { mensagem = "Sucesso" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception erro)
            {
                return Json(new { mensagem = "Ocorreu um erro! " + erro.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //metodos de editar
        [HttpPost]
        public ActionResult EditarVaga(VagaViewModel model)
        {
            try
            {
                //using (var db = new FinderJobsContext())
                //{
                //    var atualiza = db.Vagas.FirstOrDefault(vaga => vaga.Id == model.Id);
                //    if (atualiza != null)
                //    {
                //        atualiza.Empresa = model.Empresa;
                //        atualiza.Descricao = model.Descricao;
                //        atualiza.Cep = model.Cep;
                //        atualiza.Habilidades = model.Habilidades;

                //        db.SaveChanges();
                //    }
                //}

                TempData["mensagem"] = "Realizado!";
                return View("VagaEditar");
            }
            catch (Exception erro)
            {
                TempData["mensagem"] = "Ocorreu um erro!";
                throw erro;
            }
        }

        public ViewResult Candidatos()
        {
            //var FinderJobsContext = new FinderJobsContext();
            //var candidatos = FinderJobsContext.Candidatos.ToList();

            return View("Candidatos", "candidatos");
        }

        public ViewResult Habilidades()
        {
            //var FinderJobsContext = new FinderJobsContext();
            //var habilidades = FinderJobsContext.Habilidades.ToList();

            return View("Habilidades", "habilidades");
        }

        //metodos de pesquisa
        public ViewResult PesquisarVaga()
        {
            var idVaga = Request.QueryString.ToString();
            var id = int.Parse(idVaga);

            //var FinderJobsContext = new FinderJobsContext();
            //List<VagaViewModel> vaga = FinderJobsContext.Vagas.Where(vg => vg.Id == id).ToList();

            var model = new VagaViewModel();
            //if (vaga != null)
            //{
            //    foreach (var item in vaga)
            //    {
            //        model.Id = item.Id;
            //        model.Empresa = item.Empresa;
            //        model.Descricao = item.Descricao;
            //        model.Cep = item.Cep;
            //        model.Habilidades = item.Habilidades;
            //    }
            //}
            return View("VagaEditar", model);
        }

        public ViewResult PesquisarCandidato()
        {
            var idCandidato = Request.QueryString.ToString();
            var id = Int32.Parse(idCandidato);

            //var db = new FinderJobsContext();
            //List<UsuarioViewModel> usu = db.Usuarios.Where(usuario => usuario.Id == id).ToList();

            var model = new UsuarioViewModel();
            //if (usu != null)
            //{
            //    foreach (var item in usu)
            //    {
            //        model.Id = item.Id;
            //        model.Nome = item.Nome;
            //        model.Cep = item.Cep;
            //        //model.Habilidades = item.Habilidades;
            //    }
            //}
            return View("CandidatoEditar", model);
        }

        public ViewResult PesquisarHabilidade()
        {
            var idHabilidade = Request.QueryString.ToString();
            var id = Int32.Parse(idHabilidade);

            //var db = new FinderJobsContext();
            //var habilidade = db.Habilidades.Where(hb => hb.Id == id).ToList();

            var model = new HabilidadesModel();
            //if (habilidade != null)
            //{
            //    foreach (var item in habilidade)
            //    {
            //        model.Id = item.Id;
            //        model.Habilidade = item.Habilidade;
            //    }
            //}
            return View("HabilidadeEditar", model);
        }

        //metodos de excluir
        public ViewResult ExcluirVaga()
        {
            var idVaga = Request.QueryString.ToString();
            var id = Int32.Parse(idVaga);

            //var FinderJobsContext = new FinderJobsContext();
            //var vaga = FinderJobsContext.Vagas.First(vg => vg.Id == id);
            //if (vaga != null)
            //{
            //    FinderJobsContext.Vagas.Remove(vaga);
            //    FinderJobsContext.SaveChanges();
            //}

            //List<VagaViewModel> listaDeVagas = FinderJobsContext.Vagas.ToList();

            return View("Vagas", "listaDeVagas");
        }

        public ViewResult ExcluirCandidato()
        {
            var idCandidato = Request.QueryString.ToString();
            var id = Int32.Parse(idCandidato);

            //var db = new FinderJobsContext();
            //var candidato = db.Candidatos.First(cand => cand.IdUsuario == id);
            //if (candidato != null)
            //{
            //    db.Candidatos.Remove(candidato);
            //    db.SaveChanges();
            //}

            //List<CandidatoModel> listaDeCandidatos = db.Candidatos.ToList();

            return View("Candidatos", "listaDeCandidatos");
        }

        public JsonResult PesquisaDistancia(DistanciaModel model)
        {
            var idVaga = Request.QueryString[0];
            var id = Int32.Parse(idVaga);

            //var db = new FinderJobsContext();
            //List<Vaga> vaga = db.Vagas.Where(vg => vg.Id == id).ToList();

            //List<UsuarioViewModel> candidatos = db.Usuarios.ToList();

            //foreach (var item in vaga)
            //{
            //    model.CepVaga = item.Cep;
            //    if (!model.CepVaga.Contains('-'))
            //    {
            //        model.CepVaga = model.CepVaga.Insert(5, "-");
            //    }
            //}

            int distanciaAtual = model.Distancia != null ? int.Parse(model.Distancia) : 0;
            int distanciaNova = 0;

            //foreach (var item in candidatos)
            //{
            //    if (!item.Cep.Contains('-'))
            //    {
            //        item.Cep = item.Cep.Insert(5, "-");
            //    }

            //    CandidatoModel candidato = db.Candidatos.Where(usuario => usuario.IdUsuario == item.Id).ToList().FirstOrDefault();
            //    //model = PesquisarDistancia(model, item, vaga, distanciaAtual, distanciaNova, candidato.Habilidades);
            //}

            var resultado = new List<object>();

            resultado.Add(new
            {
                Nome = model.NomeCandidato,
                Distancia = model.Distancia,
                Cep = model.CepCandidato,
                Porcentagem = model.Porcentagem
            });
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarVagasDistanciaAderencia()
        {
            var idVaga = Request.QueryString[0];

            var vagas = _vagaRepository.BuscarVagas();
            var vagasViewModel = Mapper.Map<List<Domain.Entities.Vaga>, List<VagaViewModel>>(vagas);

            var usuarios = _usuarioRepository.BuscarPorTipo("Candidato");
            var usuarioViewModel = Mapper.Map<List<Domain.Entities.Usuario>, List<UsuarioViewModel>>(usuarios);

            var listaDeVagas = new List<VagaViewModel>();

            // Para cada vaga será procurado o candidato mais próximo com maior aderencia
            foreach (var vaga in vagas)
            {
                var listaDeVagasTemp = new List<VagaViewModel>();
                var vagaView = new VagaViewModel();
                vagaView.Vaga = new Vaga
                {
                    Id = vaga.Id,
                    Cep = vaga.Cep,
                    DataCadastro = vaga.DataCadastro,
                    Descricao = vaga.Descricao,
                    Empresa = vaga.Empresa,
                    Habilidades = vaga.Habilidades,
                    IdEmpresa = vaga.IdEmpresa
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
                    if (!usuario.Cep.Contains('-'))
                    {
                        usuario.Cep = usuario.Cep.Insert(5, "-");
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

        private VagaViewModel PesquisarDistancia(VagaViewModel vagaView)
        {
            var HabilidadesCandidato = new string[50];
            var HabilidadesVaga = new string[50];

            if (!string.IsNullOrEmpty(vagaView.Vaga.Habilidades))
            {
                HabilidadesVaga = vagaView.Vaga.Habilidades.Split(',');
            }

            var url = "https://maps.googleapis.com/maps/api/distancematrix/json";
            var queryString = "?origins=" + vagaView.Candidato.Cep + "&destinations=" + vagaView.Vaga.Cep + "&mode=driving";
            try
            {
                var distancia = string.Empty;
                RespostaDistancia respostaJson = null;
                var request = (HttpWebRequest)WebRequest.Create(url + queryString);
                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    distancia = reader.ReadToEnd();
                    respostaJson = JsonConvert.DeserializeObject<RespostaDistancia>(distancia);
                }

                distancia = respostaJson.rows.Count > 0 && respostaJson.rows[0].elements[0].distance != null ? respostaJson.rows[0].elements[0].distance.text : "";
                decimal porcentagem = 0, indice = 100 / HabilidadesVaga.Length;

                if (distancia.Contains(" m"))
                {
                    if (vagaView.Candidato != null && vagaView.Candidato.Habilidades != null)
                    {
                        HabilidadesCandidato = vagaView.Candidato.Habilidades.Split(',');

                        foreach (var habilidade in HabilidadesCandidato)
                        {
                            if (HabilidadesVaga.Contains(habilidade))
                            {
                                porcentagem += indice;
                            }
                        }

                        vagaView.Pesquisa = new DistanciaModel { Porcentagem = porcentagem.ToString() };
                    }
                    else
                    {
                        vagaView.Pesquisa = new DistanciaModel { Porcentagem = "0" };
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
                        HabilidadesCandidato = vagaView.Candidato.Habilidades.Split(',');
                        foreach (var habilidade in HabilidadesCandidato)
                        {
                            if (HabilidadesVaga.Contains(habilidade))
                            {
                                porcentagem += indice;
                            }
                        }
                        vagaView.Pesquisa.Porcentagem = porcentagem.ToString();
                    }
                    else
                    {
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

    }
}
