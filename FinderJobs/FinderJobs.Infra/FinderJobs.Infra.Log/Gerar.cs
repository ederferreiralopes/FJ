using log4net;
using log4net.Config;
using log4net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderJobs.Infra.Log
{
    public class Gerar
    {
        public bool LogErro(string objeto, string mensagem)
        {
            LogLog.InternalDebugging = true;
            XmlConfigurator.Configure();

            ILog log = LogManager.GetLogger(objeto);
            log.Error(mensagem);

            return true;
        }

        public ILog[] Lista()
        {
            LogLog.InternalDebugging = true;
            XmlConfigurator.Configure();

            var logs = LogManager.GetCurrentLoggers(); 

            return logs;
        }
    }
}
