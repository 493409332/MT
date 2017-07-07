using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using MT.Common.JsonUtility;
using System.Diagnostics;
using MT.Common.ConfigUtility;
using log4net.Repository;
using log4net.Config;
using System.IO;
using log4net;

namespace MT.Redis
{
    public class RedisHelper
    {

        class RedisConfig {
            public string Type { get; set; }
            public string ConnectionString { get; set; }
            public string[] ConnectionStrings { get; set; }
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection=null;
        //redis链接字符串 
        private static readonly RedisConfig RedisConnectionString = ConfigurationHelper.GetConfiguration<RedisConfig>("redis.json",   ConfigurationType.JSON);
         
        public RedisHelper() {
            if (RedisConnectionString.Type.Equals("cluster"))
            {
                LazyRedis(RedisConnectionString.ConnectionStrings);
            }
            else
            {
                LazyRedis(RedisConnectionString.ConnectionString);
            }
         

        } 

        public static void LazyRedis(string ConnectionString)
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(()=> { return RedisConnect(ConnectionString); });

        } 
        public static ConnectionMultiplexer RedisConnect(string ConnectionString) { 
            var connect = ConnectionMultiplexer.Connect(ConnectionString);
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;
            return connect;
        }

        public static void LazyRedis(params string[] ConnectionString)
        {

#if !DEBUG
            foreach (var item in ConnectionString)
            {
                lazyConnection = new Lazy<ConnectionMultiplexer>(() => { return RedisConnect(item); });
                if (RedisDB.IsConnected(""))
                    return;
            }
#endif
#if DEBUG

            foreach (var item in ConnectionString)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                lazyConnection = new Lazy<ConnectionMultiplexer>(() => { return ConnectionMultiplexer.Connect(item); });

                sw.Stop();
                TimeSpan ts2 = sw.Elapsed;
                sw.Restart();


            

                if (RedisDB.IsConnected(""))
                {
                    sw.Stop();
                    ts2 = sw.Elapsed;
                    return;
                }
                sw.Stop();
                ts2 = sw.Elapsed;
            }
#endif
        }


        public static IDatabase RedisDB
        {
            get
            {  
                return lazyConnection.Value.GetDatabase();
            }
        }


#region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {

            ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            var file = new FileInfo("log4net.config");
            XmlConfigurator.Configure(repository, file);



            ILog log = LogManager.GetLogger(repository.Name, "testApp.Logging");

            log.Info("Configuration changed: " + e.EndPoint);
 
             
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine("ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine("InternalError:Message" + e.Exception.Message);
        }



#endregion 事件

        public bool Set<T>(string Key, T t, TimeSpan ts)
        {

            var tstr = JsonConvert.SerializeObject(t);
            return RedisDB.StringSet(Key, tstr, ts);
        }
        public bool Set<T>(string Key, T t)
        {

            var tstr = JsonConvert.SerializeObject(t);
            return RedisDB.StringSet(Key, tstr);
        }

        public T Get<T>(string Key) where T : class
        {
            var strValue = RedisDB.StringGet(Key).ToString();

            if (!string.IsNullOrEmpty(strValue))
            {
                if (JsonHelper.IsJson(strValue))
                {
                    return JsonConvert.DeserializeObject<T>(strValue);
                }
                return strValue as T;
            }
            return null;

        }

        //public static string ListKeyName = "MessageList";
        //public void HomeController()
        //{
        //   var db = Connection.GetDatabase();
        //    if (db.IsConnected(ListKeyName) && (!db.KeyExists(ListKeyName) || !db.KeyType(ListKeyName).Equals(RedisType.List)))
        //    {
        //        //Add sample data.
        //        db.KeyDelete(ListKeyName);
        //        //Push data from the left
        //        db.ListLeftPush(ListKeyName, "TestMsg1");
        //        db.ListLeftPush(ListKeyName, "TestMsg2");
        //        db.ListLeftPush(ListKeyName, "TestMsg3");
        //        db.ListLeftPush(ListKeyName, "TestMsg4");
        //    }



        //}

    }


}
