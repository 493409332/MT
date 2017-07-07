using MT.Common.ConfigUtility;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MT.Redis
{
    /// <summary>
    /// redis链接帮助类
    /// </summary>
    public  class RedisConnectionHelp
    {
      
        //redis链接字符串 
        private static readonly string RedisConnectionString = ConfigurationHelper.GetConfigurationValue<string>("test.xml", "connectionString", ConfigurationType.XML);
        //锁
        private static readonly object Locker = new object();
        private static ConnectionMultiplexer _instance;

    }
}
