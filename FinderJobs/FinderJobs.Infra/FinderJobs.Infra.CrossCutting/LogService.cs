using FinderJobs.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Infra.CrossCutting
{
    public static class LogService
    {
        public static IEnumerable<Log> Get(string tipo)
        {
            return GetLogCollection().Find(x => x.Tipo == tipo).ToEnumerable();
        }

        public static bool Desativar(Guid id)
        {
            return GetLogCollection().UpdateOne(Builders<Log>.Filter.Eq("_id", id), Builders<Log>.Update.Set("Ativo", false)).ModifiedCount > 0;
        }

        public static void NotifyException(string functionName, Exception ex)
        {
            string allInfo = functionName;
            var tipo = string.Empty;
            var objeto = string.Empty;
            if (ex != null)
            {
                allInfo = GetAllInformation(ex);
                objeto = ex.Source != null ? ex.Source : "";
                tipo = ex != null ? ex.GetType().ToString() : "";                
            }     
             
            GetLogCollection().InsertOneAsync(new Log { Ativo = true, Data = DateTime.Now, Tipo = tipo, Objeto = objeto, Mensagem = allInfo });
        }

        public static void NotifyInfo(string functionName, string Info)
        {
            GetLogCollection().InsertOneAsync(new Log { Ativo = true, Data = DateTime.Now, Tipo = "Info", Objeto = functionName, Mensagem = Info });
        }

        public static string GetAllInformation(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine("********** " + DateTime.Now.ToLongDateString() + " **********");

            if (exception != null)
            {

                if (exception.InnerException != null)
                {
                    sb.AppendLine("Inner Exception Type: ");
                    sb.AppendLine(exception.InnerException.GetType().ToString());

                    if (exception.InnerException.Message != null)
                    {
                        sb.AppendLine("Inner Exception: ");
                        sb.AppendLine(exception.InnerException.Message);
                    }
                    if (exception.InnerException.Source != null)
                    {
                        sb.AppendLine("Inner Source: ");
                        sb.AppendLine(exception.InnerException.Source);
                    }

                    if (exception.InnerException.StackTrace != null)
                    {
                        sb.AppendLine("Inner Stack Trace: ");
                        sb.AppendLine(exception.InnerException.StackTrace);
                    }
                }                                                      

                if (exception.Message != null)
                {
                    sb.AppendLine("Exception Type: ");
                    sb.AppendLine(sb.GetType().ToString());
                    sb.AppendLine("Exception: " + exception.Message);
                }
                                        
                if (exception.StackTrace != null)
                {
                    sb.AppendLine("Stack Trace: ");
                    sb.AppendLine(exception.StackTrace);
                    sb.AppendLine();
                }                
            }

            return sb.ToString();
        }

        private static IMongoCollection<Log> GetLogCollection()
        {
            var databaseName = ConfigurationManager.AppSettings.Get("MongoDbDatabaseName");
            var connString = ConfigurationManager.AppSettings.Get("MongoDbConnectionString").Replace("{DB_NAME}", databaseName);            
            var client = new MongoClient(connString);
            var database = client.GetDatabase(databaseName);

            return database.GetCollection<Log>(typeof(Log).Name);
        }
    }
}
