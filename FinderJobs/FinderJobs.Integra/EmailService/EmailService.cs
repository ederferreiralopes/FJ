using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinderJobs.EmailService
{
    public abstract class EntityBase
    {
        [BsonId]
        public Guid Id { get; set; }
        public bool Ativo { get; set; }
    }

    public class Log : EntityBase
    {
        public string Tipo { get; set; }
        public string Mensagem { get; set; }
        public object Objeto { get; set; }
        public DateTime Data { get; set; }
    }

    public class Email : EntityBase
    {
        public string Remetente { get; set; }
        public string Mensagem { get; set; }
        public string Destino { get; set; }
        public string Titulo { get; set; }        
        public string TipoDestino { get; set; }
    }

    public class EmailTemplate : EntityBase
    {
        public string TipoDestino { get; set; }
        public string Conteudo { get; set; }
    }

    public partial class EmailService : ServiceBase
    {
        Timer timer;
        int intervalo;

        public EmailService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int periodoEnviarEmailMinutos = int.Parse(ConfigurationManager.AppSettings.Get("PeriodoEnviarEmailMinutos"));
            intervalo = periodoEnviarEmailMinutos * 60 * 1000;
            timer = new Timer(new TimerCallback(intervalo_Tick), null, 0, intervalo);
        }

        protected override void OnStop()
        {
            GetLogCollection().InsertOneAsync(new Log { Ativo = true, Tipo = "Info", Data = DateTime.Now, Mensagem = "Servico Parado: " + DateTime.Now.ToString(), Objeto = "Serviço de Email" });
        }

        public void intervalo_Tick(object sender)
        {
            var getLogCollection = GetLogCollection();
            getLogCollection.InsertOneAsync(new Log { Ativo = true, Tipo = "Info", Data = DateTime.Now, Mensagem = "Servico Rodando em " + DateTime.Now.ToString() + " ---> Intervalo definido em milisegundos: " + intervalo, Objeto = "Integração PagSeguro" });
            EnviarEmail();
        }

        public static void EnviarEmail()
        {
            var getLogCollection = GetLogCollection();
            var getEmailCollection = GetEmailCollection();
            var getEmailTemplateCollection = GetEmailTemplateCollection();

            try
            {
                var emails = getEmailCollection.Find(x => x.Ativo).ToList();
                var templateEmailCand = new EmailTemplate();
                var templateEmailEmpr = new EmailTemplate();

                foreach (var item in emails)
                {
                    if (!string.IsNullOrWhiteSpace(item.TipoDestino))
                    {
                        switch (item.TipoDestino)
                        {
                            case "Candidato":
                                templateEmailCand = templateEmailCand == null ? getEmailTemplateCollection.Find(x => x.Ativo && x.TipoDestino == "Candidato").FirstOrDefault() : templateEmailCand;
                                item.Mensagem = templateEmailCand.Conteudo.Replace("#mensagemTitulo", item.Titulo).Replace("#mensagemTexto", item.Mensagem);
                                break;
                            case "Empresa":
                                templateEmailEmpr = templateEmailEmpr == null ? getEmailTemplateCollection.Find(x => x.Ativo && x.TipoDestino == "Empresa").FirstOrDefault() : templateEmailEmpr;
                                item.Mensagem = templateEmailEmpr.Conteudo.Replace("#mensagemTitulo", item.Titulo).Replace("#mensagemTexto", item.Mensagem);
                                break;
                            default:
                                getLogCollection.InsertOneAsync(new Log { Ativo = true, Tipo = "Erro", Data = DateTime.Now, Mensagem = "Serviço de Email - Template não definido para o email com id " + item.Id, Objeto = "FinderJobs.Integra.EmailService" });
                                break;
                        }
                    }
                    SendAsync(item);
                    var resultado = getEmailCollection.UpdateOne(Builders<Email>.Filter.Eq("_id", item.Id), Builders<Email>.Update.Set("Ativo", "false"));
                }

                getLogCollection.InsertOneAsync(new Log { Ativo = true, Tipo = "Info", Data = DateTime.Now, Mensagem = "Encontrados " + emails.Count() + " emails em : " + DateTime.Now.ToString(), Objeto = "Serviço de Email" });
            }
            catch (Exception ex)
            {
                getLogCollection.InsertOneAsync(new Log { Ativo = true, Tipo = "Erro", Data = DateTime.Now, Mensagem = "Serviço de Email " + ex.Message, Objeto = ex.Source });
            }
        }

        public static Task SendAsync(Email email)
        {
            var emailCredencial = ConfigurationManager.AppSettings.Get("EmailCredencial");
            var emailSenha = ConfigurationManager.AppSettings.Get("EmailSenha");
            var emailHost = ConfigurationManager.AppSettings.Get("EmailHostSmtp");

            SmtpClient client = new SmtpClient();
            client.Port = int.Parse(ConfigurationManager.AppSettings.Get("EmailHostPort") ?? "0");
            client.Host = emailHost;
            client.Timeout = 24000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(emailCredencial, emailSenha);
            var mailMessage = new MailMessage();

            mailMessage.To.Add(email.Destino);
            mailMessage.Subject = email.Titulo;
            mailMessage.Body = email.Mensagem;
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress(emailCredencial);

            return client.SendMailAsync(mailMessage);
        }

        private static IMongoCollection<Log> GetLogCollection()
        {
            return GetMongoDatabase().GetCollection<Log>("Log");
        }

        private static IMongoCollection<Email> GetEmailCollection()
        {
            return GetMongoDatabase().GetCollection<Email>("Email");
        }

        private static IMongoCollection<EmailTemplate> GetEmailTemplateCollection()
        {
            return GetMongoDatabase().GetCollection<EmailTemplate>("EmailTemplate");
        }

        private static IMongoDatabase GetMongoDatabase()
        {
            var databaseName = ConfigurationManager.AppSettings.Get("MongoDbDatabaseName");
            var connString = ConfigurationManager.AppSettings.Get("MongoDbConnectionString").Replace("{DB_NAME}", databaseName);
            var client = new MongoClient(connString);
            return client.GetDatabase(databaseName);
        }
    }
}
