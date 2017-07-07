using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MT.Redis
{
     public   class Log4Net
    {

        public static void Logtest()
        {
            ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            var file = new FileInfo("log4net.config");
            XmlConfigurator.Configure(repository, file);



            ILog log = LogManager.GetLogger(repository.Name, "testApp.Logging");
         
            log.Info("NETCorelog4net log");
            log.Info("test log");
            log.Error("error");
            log.Info("linezero");
            
        }
    }
}
