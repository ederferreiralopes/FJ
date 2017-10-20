using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Exception;
using Uol.PagSeguro.Resources;
using Uol.PagSeguro.Service;

namespace Teste
{
    public abstract class EntityBase
    {
        [BsonId]
        public Guid Id { get; set; }
        public bool Ativo { get; set; }
    }

    public class Pagamento : EntityBase
    {
        public string Codigo { get; set; }
        public string Referencia { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public DateTime DataCadastro { get; set; }
    }

    public class Email : EntityBase
    {
        public string Remetente { get; set; }
        public string Mensagem { get; set; }
        public string Destino { get; set; }
        public string Titulo { get; set; }
        public Guid IdVaga { get; set; }
        public string TipoDestino { get; set; }
    }

    public class Log : EntityBase
    {
        public string Tipo { get; set; }
        public string Mensagem { get; set; }
        public object Objeto { get; set; }
        public DateTime Data { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //SincroniarPagSeguro();

            EnviarEmail();
        }

        private static IMongoCollection<Log> GetLogCollection()
        {
            return GetMongoDatabase().GetCollection<Log>("Log");
        }

        private static IMongoCollection<Pagamento> GetPagtoCollection()
        {
            return GetMongoDatabase().GetCollection<Pagamento>("Pagamento");
        }

        private static IMongoDatabase GetMongoDatabase()
        {
            var databaseName = ConfigurationManager.AppSettings.Get("MongoDbDatabaseName");
            var connString = ConfigurationManager.AppSettings.Get("MongoDbConnectionString").Replace("{DB_NAME}", databaseName);
            var client = new MongoClient(connString);
            return client.GetDatabase(databaseName);
        }

        public static void SincroniarPagSeguro()
        {
            var getLogCollection = GetLogCollection();
            var periodoConsultaPagseguro = int.Parse(ConfigurationManager.AppSettings.Get("PeriodoConsultaPagseguroDias"));
            var isSandbox = EnvironmentConfiguration.ChangeEnvironment();

            try
            {
                AccountCredentials credentials = PagSeguroConfiguration.Credentials(isSandbox);
                var dataInicio = DateTime.Now.AddDays(-periodoConsultaPagseguro);
                var inicio = new DateTime(dataInicio.AddDays(-1).Year, dataInicio.AddDays(-1).Month, dataInicio.AddDays(-1).Day, 0, 0, 0);
                var fim = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.Month, DateTime.Now.Day - 1, 23, 59, 59);

                var trans = TransactionSearchService.SearchByDate(credentials, inicio, fim, false);
                var pagtos = from p in trans.Transactions select new Pagamento { Codigo = p.Code, Referencia = p.Reference, Valor = p.GrossAmount, Status = p.TransactionStatus.ToString(), DataCadastro = p.Date };
                var getPagtoCollection = GetPagtoCollection();
                foreach (var item in pagtos)
                {
                    var resultado = getPagtoCollection.UpdateOne(Builders<Pagamento>.Filter.Eq("Referencia", item.Referencia), Builders<Pagamento>.Update.Set("Status", item.Status));
                    if (resultado != null && resultado.MatchedCount == 0)
                        getPagtoCollection.InsertOneAsync(item);
                }

                getLogCollection.InsertOneAsync(new Log { Ativo = true, Tipo = "Info", Data = DateTime.Now, Mensagem = "Encontrados " + pagtos.Count() + " pagamentos em : " + DateTime.Now.ToString(), Objeto = "Integração PagSeguro" });
            }
            catch (PagSeguroServiceException ex)
            {
                var errorMsg = ex.Message + "\n";

                foreach (ServiceError element in ex.Errors)
                {
                    errorMsg += element + "\n";
                }

                getLogCollection.InsertOneAsync(new Log { Ativo = true, Tipo = "Erro - " + (isSandbox ? "Teste" : "Produção"), Data = DateTime.Now, Mensagem = "Integração PagSeguro " + errorMsg, Objeto = ex.Source });
            }
        }

        public static void EnviarEmail()
        {
            var getLogCollection = GetLogCollection();
            var getEmailCollection = GetEmailCollection();

            try
            {
                var emails = getEmailCollection.Find(x => x.Ativo).ToList();
                foreach (var item in emails)
                {
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

        private static IMongoCollection<Email> GetEmailCollection()
        {
            return GetMongoDatabase().GetCollection<Email>("Email");
        }

        public static Task SendAsync(Email email)
        {
            var emailCredencial = ConfigurationManager.AppSettings.Get("EmailCredencial");
            var emailSenha = ConfigurationManager.AppSettings.Get("EmailSenha");
            var emailHost = ConfigurationManager.AppSettings.Get("EmailHostSmtp");

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = emailHost;
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(emailCredencial, emailSenha);
            var mailMessage = new MailMessage();

            mailMessage.To.Add(email.Destino);
            mailMessage.Subject = email.Titulo;
            mailMessage.Body = email.Mensagem;
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("contato@finderJobs.com.br");

            return client.SendMailAsync(mailMessage);
        }
    }
}
