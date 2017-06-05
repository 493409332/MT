using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Xml;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Configuration.Binder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MT.Common.ConfigHelper
{
 
    public enum ConfigurationType
    {
        XML,
        JSON,
        INI,
        Memory
    }
    public class ConfigurationHelper
    {
        static ConfigurationBuilder configbuilder = new ConfigurationBuilder();

      
        /// <summary>
        /// 根据节点获取配置
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <param name="key">节点 key ; key:value</param>
        /// <param name="BasePath">文件目录</param>
        /// <returns></returns>
        public static T GetConfigurationValue<T>(string FileName, string key, ConfigurationType configtype = ConfigurationType.JSON, string BasePath = "") 
        {
            string BasePat = BasePath.Length == 0 ? Directory.GetCurrentDirectory() : BasePath;
            switch (configtype)
            {
                case ConfigurationType.XML:
                    return new ConfigurationBuilder().SetBasePath(BasePat).AddInMemoryCollection().AddXmlFile(FileName).Build().GetValue<T>(key);
                case ConfigurationType.JSON:
                    return new ConfigurationBuilder().SetBasePath(BasePat).AddJsonFile(FileName).Build().GetValue<T>(key);
                case ConfigurationType.INI:
                    return new ConfigurationBuilder().SetBasePath(BasePat).AddIniFile(FileName).Build().GetValue<T>(key);
                case ConfigurationType.Memory:
                    return configbuilder.AddInMemoryCollection().Build().GetValue<T>(key);
                default:
                    return default(T);
            }
 
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <param name="key">节点 key ; key:value</param>
        /// <param name="BasePath">文件目录</param>
        /// <returns></returns>
        public static T GetConfiguration<T>(string FileName,  ConfigurationType configtype = ConfigurationType.JSON, string BasePath = "")
        {
            string BasePat = BasePath.Length == 0 ? Directory.GetCurrentDirectory() : BasePath;
            switch (configtype)
            {
                case ConfigurationType.XML:
                    return new ConfigurationBuilder().SetBasePath(BasePat).AddInMemoryCollection().AddXmlFile(FileName).Build().Get<T>();
                case ConfigurationType.JSON:
                    return new ConfigurationBuilder().SetBasePath(BasePat).AddJsonFile(FileName).Build().Get<T>();
                case ConfigurationType.INI:
                    return new ConfigurationBuilder().SetBasePath(BasePat).AddIniFile(FileName).Build().Get<T>();
                case ConfigurationType.Memory:
                    return configbuilder.AddInMemoryCollection().Build().Get<T>();
                default:
                    return default(T);
            }

        }
        public static void SetMemoryConfiguration(List<KeyValuePair<string, string>> Data)
        {
            var defaultSettings = new MemoryConfigurationSource();
            defaultSettings.InitialData = Data;
            configbuilder.Add(defaultSettings);

         
        }

    }
}
