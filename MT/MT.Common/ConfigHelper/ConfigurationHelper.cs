using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace MT.Common.ConfigHelper
{
    /// <summary>
    /// 配置类型
    /// </summary>
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
        /// <typeparam name="T">支持所有类型</typeparam>
        /// <param name="FileName">文件名</param>
        /// <param name="key">节点 key ; key:value</param>
        /// <param name="configtype">配置类型</param>
        /// <param name="BasePath">文件目录</param>
        /// <returns>返回对应泛型值</returns>
        public static T GetConfigurationValue<T>(string FileName, string key, ConfigurationType configtype = ConfigurationType.JSON, string BasePath = "")
        {
            string BasePat = BasePath.Length == 0 ? Directory.GetCurrentDirectory() : BasePath;
            Type type = typeof(T);

            if (type.GetTypeInfo().IsValueType)
            {
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
            else if (type.IsConstructedGenericType)
            {
                T list = Activator.CreateInstance<T>();
                int index = 0;
                bool IsRun = true;
                do
                {
                    Type Generic = type.GetGenericArguments().First();

                    object model = typeof(ConfigurationHelper).GetMethod("GetConfigurationValue").MakeGenericMethod(Generic).Invoke(null, new object[] { FileName, string.Format("{0}:{1}", key, index), configtype, "" });
                    if (model != null)
                    {
                        index++;
                        type.GetMethod("Add").Invoke(list, new object[] { model });
                    }
                    else 
                        IsRun = false;  
                } while (IsRun);

                return list;
            }
            else
            {
                T model = Activator.CreateInstance<T>();
                bool isnull = false;
                foreach (var item in type.GetProperties())
                {
                    object defaultval = null;

                    object val = typeof(ConfigurationHelper).GetMethod("GetConfigurationValue").MakeGenericMethod(item.PropertyType).Invoke(null, new object[] { FileName, string.Format("{0}:{1}", key, item.Name), configtype, "" });

                    if (item.PropertyType.GetTypeInfo().IsValueType)
                    {
                        defaultval = Activator.CreateInstance(item.PropertyType);
                        if (val != defaultval && !val.Equals(defaultval))
                            isnull = true;
                    }
                    item.SetValue(model, val);
                }
                if (isnull)
                    return model;
                else
                    return default(T);

            } 

        }

 
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T">对应类型整体转换</typeparam>
        /// <param name="FileName">文件名</param>
        /// <param name="configtype">配置类型</param>
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
