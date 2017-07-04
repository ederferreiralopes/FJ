using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Exception;
using Uol.PagSeguro.Resources;
using Uol.PagSeguro.Service;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace FinderJobs.Integra
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

    public class Log : EntityBase
    {
        public string Tipo { get; set; }
        public string Mensagem { get; set; }
        public object Objeto { get; set; }
        public DateTime Data { get; set; }
    }

    public class Email : EntityBase
    {
        public virtual string Mensagem { get; set; }
        public virtual string Destino { get; set; }
        public virtual string Titulo { get; set; }
    }

    public partial class IntegraPagseguro : ServiceBase
    {
        Timer timer;
        int intervalo;

        public IntegraPagseguro()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int periodoConsultaPagseguro = int.Parse(ConfigurationManager.AppSettings.Get("PeriodoConsultaPagseguroDias"));
            intervalo = periodoConsultaPagseguro * 24 * 60 * 60 * 1000;
            timer = new Timer(new TimerCallback(intervalo_Tick), null, 0, intervalo);
        }

        protected override void OnStop()
        {

            GetLogCollection().InsertOneAsync(new Log { Ativo = true, Tipo = "Info", Data = DateTime.Now, Mensagem = "Servico Parado: " + DateTime.Now.ToString(), Objeto = "Integração PagSeguro" });
        }

        public void intervalo_Tick(object sender)
        {
            var getLogCollection = GetLogCollection();
            getLogCollection.InsertOneAsync(new Log { Ativo = true, Tipo = "Info", Data = DateTime.Now, Mensagem = "Servico Rodando em " + DateTime.Now.ToString() + " ---> Intervalo definido em milisegundos: " + intervalo, Objeto = "Integração PagSeguro" });
            SincroniarPagSeguro();
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
                
                getLogCollection.InsertOneAsync(new Log { Ativo = true, Tipo = "Erro", Data = DateTime.Now, Mensagem = "Integração PagSeguro " + errorMsg, Objeto = ex.Source });
            }
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
    }
}
