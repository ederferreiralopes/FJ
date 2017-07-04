using FinderJobs.Application;
using FinderJobs.Application.Interface;
using FinderJobs.Domain.Interfaces.Services;
using FinderJobs.Site.Services;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FinderJobs.Site
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            try
            {
                var url = "http://localhost/Manager/Account/LoginApi?";
                var parametros = string.Concat("Email=", context.UserName, "&Password=", context.Password);
                var respostaJson = string.Empty;
                var request = (HttpWebRequest)WebRequest.Create(url + parametros);
                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    respostaJson = reader.ReadToEnd();
                }

                var resposta = JsonConvert.DeserializeAnonymousType(respostaJson, new { sucesso = false, mensagem = "", roles = "" });
                if (resposta == null || !resposta.sucesso)
                {
                    context.SetError("invalid_grant", "Usuário ou senha inválidos");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                var roles = new List<string>();
                roles.Add("Usuario");
                roles.AddRange(resposta.roles.Split(','));

                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                GenericPrincipal principal = new GenericPrincipal(identity, roles.ToArray());
                Thread.CurrentPrincipal = principal;

                context.Validated(identity);
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", "falha ao autenticar");
            }
        }
    }
}