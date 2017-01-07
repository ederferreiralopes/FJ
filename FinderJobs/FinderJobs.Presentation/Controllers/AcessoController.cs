using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FinderJobs.MVC.ViewModels;
using AutoMapper;
using Newtonsoft.Json;
using FinderJobs.Application.Interface;
using System.IO;
using FinderJobs.Domain.Entities;

namespace FinderJobs.MVC.Controllers
{
    public class AcessoController : Controller
    {

        private readonly IUsuarioAppService _usuarioService;
        private readonly IHabilidadeAppService _habilidadeService;
        private readonly IArquivoAppService _arquivoService;
        private readonly IConfiguracaoBoletoAppService _configuracaoBoletoService;

        public AcessoController(IUsuarioAppService usuarioService, IHabilidadeAppService habilidadeService, IArquivoAppService arquivoAppService, IConfiguracaoBoletoAppService configuracaoBoletoService)
        {
            _usuarioService = usuarioService;
            _habilidadeService = habilidadeService;
            _arquivoService = arquivoAppService;
            _configuracaoBoletoService = configuracaoBoletoService;
        }

        public ActionResult Index()
        {            
            return File("~/views/Index.html", "text/html");
        }


        public ActionResult Cadastro()
        {
            return File("~/views/Cadastro.html", "text/html");
        }

        [HttpPost]
        public ActionResult Login(UsuarioViewModel model)
        {
            var usuario = _usuarioService.ValidarAcesso(model.Login, model.Senha);
            if (usuario != null)
            {
                var usuarioViewModel = Mapper.Map<Domain.Entities.Usuario, UsuarioViewModel>(usuario);
                usuarioViewModel.Senha = string.Empty;
                var authenticationTicket = new FormsAuthenticationTicket(usuarioViewModel.Login, false, 60);
                string encryptTicket = FormsAuthentication.Encrypt(authenticationTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptTicket);
                Response.Cookies.Add(authCookie);

                return Json(new { sucesso = true, usuario = usuarioViewModel }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UsuarioDisponivel(string login)
        {
            var disponivel = false;
            if (!string.IsNullOrWhiteSpace(login))
                disponivel = !_usuarioService.ValidarLogin(login);

            return Json(new { sucesso = disponivel }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Cadastrar(UsuarioViewModel model)
        {
            try
            {
                var retorno = string.Empty;
                var sucesso = false;

                if (!string.IsNullOrWhiteSpace(model.Login) && !_usuarioService.ValidarLogin(model.Login))
                {
                    var habilidades = JsonConvert.DeserializeObject<List<HabilidadeJson>>(model.Habilidades);
                    var habilidadesExistentes = habilidades.Where(x => !x.Id.Equals(x.Nome)).ToList();

                    // Grava Habilidades Novas
                    foreach (var item in habilidades.Where(x => x.Id.Equals(x.Nome)))
                    {
                        var id = (int)_habilidadeService.Add(new Domain.Entities.Habilidade { Nome = item.Nome });
                        habilidadesExistentes.Add(new HabilidadeJson { Id = id.ToString(), Nome = item.Nome });
                    }

                    var usuario = Mapper.Map<UsuarioViewModel, Domain.Entities.Usuario>(model);
                    usuario.DataCadastro = DateTime.Now.ToString();
                    usuario.Ativo = true;
                    usuario.Habilidades = JsonConvert.SerializeObject(habilidadesExistentes);

                    usuario.Id = (int)_usuarioService.Add(usuario);


                    if (usuario.Id > 0)
                        sucesso = true;

                    return Json(new { usuario = usuario, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { sucesso = sucesso, mensagem = "Usuário inválido ou indisponível" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Alterar(UsuarioViewModel model)
        {
            try
            {
                var retorno = string.Empty;
                var sucesso = false;

                if (model.Id > 0 && !string.IsNullOrWhiteSpace(model.Login))
                {
                    var usuario = _usuarioService.GetById(model.Id);
                    if (usuario != null && usuario.Login.Equals(model.Login))
                    {
                        var habilidades = JsonConvert.DeserializeObject<List<HabilidadeJson>>(model.Habilidades);
                        var habilidadesExistentes = habilidades.Where(x => !x.Id.Equals(x.Nome)).ToList();

                        // Grava Habilidades Novas
                        foreach (var item in habilidades.Where(x => x.Id.Equals(x.Nome)))
                        {
                            var id = (int)_habilidadeService.Add(new Domain.Entities.Habilidade { Nome = item.Nome });
                            habilidadesExistentes.Add(new HabilidadeJson { Id = id.ToString(), Nome = item.Nome });
                        }

                        usuario.Nome = model.Nome;
                        usuario.Celular = model.Celular;
                        usuario.CpfCnpj = model.CpfCnpj;
                        usuario.Email = model.Email;
                        usuario.EnderecoCep = model.EnderecoCep;
                        usuario.EnderecoLogradouro = model.EnderecoLogradouro;
                        usuario.EnderecoNumero = model.EnderecoNumero;
                        usuario.EnderecoBairro = model.EnderecoBairro;
                        usuario.EnderecoCidade = model.EnderecoCidade;
                        usuario.EnderecoUF = model.EnderecoUF;
                        usuario.Pago = model.Pago;
                        usuario.Anonimo = model.Anonimo;
                        usuario.Habilidades = JsonConvert.SerializeObject(habilidadesExistentes);

                        _usuarioService.Update(usuario);

                        if (usuario.Id > 0)
                            sucesso = true;

                        return Json(new { usuario = usuario, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { sucesso = sucesso, mensagem = "Usuário não encontrado" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { sucesso = sucesso, mensagem = "Não foi possível atualizar seus dados" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GravarArquivo(int id, string tipo, HttpPostedFileBase fileUpload)
        {
            try
            {
                var retorno = string.Empty;
                var sucesso = false;

                var usuarioTipo = (UsuarioTipo)Enum.Parse(typeof(UsuarioTipo), tipo, true);                
                
                switch (usuarioTipo)
                {
                    case UsuarioTipo.Candidato: tipo = string.Concat("/", ArquivoLocal.Curriculo.ToString(), "/");
                        break;
                    case UsuarioTipo.Empresa: tipo = string.Concat("/", ArquivoLocal.Vaga.ToString(), "/");
                        break;
                    default: tipo = string.Concat("/", ArquivoLocal.Indefinido.ToString(), "/");
                        break;
                }

                var caminho = "~/Arquivo/" + id + tipo;
                var nome = fileUpload.FileName.Length > 50 ? fileUpload.FileName.Substring(fileUpload.FileName.Length - 50) : fileUpload.FileName;

                var arquivo = new Domain.Entities.Arquivo { Usuario = new Domain.Entities.Usuario { Id = id }, Caminho = caminho, Nome = nome, Tipo = tipo, Ativo = true };
                if (!System.IO.Directory.Exists(Server.MapPath(caminho)))
                    System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                fileUpload.SaveAs(Server.MapPath(caminho) + nome);

                _arquivoService.Desativar(id);
                var idArquivo = _arquivoService.Add(arquivo);

                if (idArquivo != null)
                    sucesso = true;

                return Json(new { id = idArquivo, url = caminho + "/" + nome, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CarregarArquivo(int id = 0)
        {
            try
            {
                var retorno = string.Empty;
                var sucesso = false;

                var arquivo = _arquivoService.GetArquivo(id, ArquivoTipo.Curriculo.ToString()).ToList();
                if (arquivo != null && arquivo.Count > 0)                
                    sucesso = true;

                return Json(new { arquivo = arquivo, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExcluirArquivo(int id)
        {
            try
            {
                _arquivoService.Desativar(id);

                return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GerarBoleto(UsuarioViewModel usuario)
        {
            var boleto = new BoletoNet.BoletoBancario();
            var model = new BoletoModel();
            var configuracaoBoleto = _configuracaoBoletoService.GetById(1);
            model.Id = configuracaoBoleto.Id;
            model.CodigoBanco = configuracaoBoleto.CodigoBanco;
            model.CodigoCarteira = configuracaoBoleto.CodigoCarteira;
            model.MostrarCodigoCarteira = configuracaoBoleto.MostrarCodigoCarteira;
            model.MostrarComprovanteEntrega = configuracaoBoleto.MostrarComprovanteEntrega;
            model.NumeroDocumento = configuracaoBoleto.NumeroDocumento;
            model.ValorBoleto = configuracaoBoleto.ValorBoleto;
            model.Vencimento = configuracaoBoleto.Vencimento;

            model.Cedente = new CedenteModel
            {
                Id = configuracaoBoleto.Cedente.Id,
                Nome = configuracaoBoleto.Cedente.Nome,
                CpfCnpj = configuracaoBoleto.Cedente.CpfCnpj,
                NossoNumero = configuracaoBoleto.Cedente.NossoNumero,
                Agencia = configuracaoBoleto.Cedente.Agencia,
                Conta = configuracaoBoleto.Cedente.Conta,
                DigitoConta = configuracaoBoleto.Cedente.DigitoConta,
                Codigo = configuracaoBoleto.Cedente.Codigo

            };

            model.Sacado = new SacadoModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                CpfCnpj = usuario.CpfCnpj,
                Cep = usuario.EnderecoCep,
                Endereco = usuario.EnderecoLogradouro,
                Bairro = usuario.EnderecoBairro,
                Cidade = usuario.EnderecoCidade,
                Uf = usuario.EnderecoUF
            };

            // criar entidade Assinatura
            // migrar isso para MongoDB
            // depois de gerar o boleto gravar o html no MongoDB

            if (model != null && model.Cedente.Id > 0 && model.Sacado.Id > 0)
                boleto = new Services.Boleto().GeraBoleto(model);

            var retorno = string.Empty;
            var sucesso = false;
            var idArquivo = 0;
            var caminho = string.Empty;;
            var nome = string.Empty;;
            
            if (boleto != null)
            {
                try
                {
 
                    caminho = "~/Arquivo/" + usuario.Id + "/Boleto/";
                    nome = boleto.Boleto.NumeroDocumento + ".pdf";
                    byte[] arquivoPDF = boleto.MontaBytesPDF();
                    var arquivo = new Domain.Entities.Arquivo { Usuario = new Domain.Entities.Usuario { Id = usuario.Id }, Caminho = caminho, Nome = nome, Tipo = "Boleto", Ativo = true };

                    if (!System.IO.Directory.Exists(Server.MapPath(caminho)))
                        System.IO.Directory.CreateDirectory(Server.MapPath(caminho));

                    //Crio o arquivo em disco e um fluxo
                    FileStream Stream = new FileStream(Server.MapPath(caminho) + nome, FileMode.Create);
                    Stream.Write(arquivoPDF, 0, arquivoPDF.Length);

                    //_arquivoService.Desativar(id);
                    idArquivo = (int)_arquivoService.Add(arquivo);

                    if (idArquivo != null)
                        sucesso = true;                    
                }
                catch (Exception ex)
                {
                    return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { id = idArquivo, url = caminho + "/" + nome, sucesso = sucesso }, JsonRequestBehavior.AllowGet);
        }

        public RedirectToRouteResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();

            return RedirectToAction("Index");
        }
    }

}
