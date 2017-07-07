using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MT.Common.Log4Utility
{
    /// <summary>
    /// log4帮助类  输出日志请不要含有特殊字符▆
    /// </summary>
    public class Log4Helper
    {
        readonly static ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
        /// <summary>
        /// 单利模式
        /// </summary>
        public readonly static Log4Helper Instance = new Log4Helper();
        public Log4Helper()
        {
            var file = new FileInfo("log4net.config");
            XmlConfigurator.Configure(repository, file);
        }
        /// <summary>
        /// 支持索引方式
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ILog this[Log4level index]
        {
            get
            {
                return LogManager.GetLogger(repository.Name, index.ToString()); ;
            }
        }
        /// <summary>
        ///获取日志接口
        /// </summary>
        /// <param name="index">日志等级</param>
        /// <returns></returns>
        public static ILog GetLog(Log4level index)
        {

            return Instance.GetILog(index);
        }
        /// <summary>
        /// 获取日志接口
        /// </summary>
        /// <param name="index">日志等级</param>
        /// <returns></returns>
        public ILog GetILog(Log4level index)
        {
            return LogManager.GetLogger(repository.Name, index.ToString()); ;
        }

        public static List<LogInfo> ReadLogList(string path)
        {

            var file = new FileInfo(path);
            List<LogInfo> list = new List<LogInfo>();
            try
            {


                using (var fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string temp = string.Empty;

                        while ((temp = sr.ReadLine()) != null)
                        {
                            string[] modelstr = temp.Split('▆');
                            LogInfo model = new LogInfo();
                            foreach (var item in modelstr)
                            {
                                if (item.StartsWith("记录时间:"))
                                {
                                    model.Date = DateTime.ParseExact(item.Replace("记录时间:", ""), "yyyy-MM-dd hh:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture); 
                                }
                                if (item.StartsWith("线程ID:"))
                                {
                                    model.Thread = int.Parse(item.Replace("线程ID:", "").Replace("[","").Replace("]", ""));
                                }
                                if (item.StartsWith("日志级别:"))
                                {
                                    model.Level = item.Replace("日志级别:", "");
                                }
                                if (item.StartsWith("LoggerName:"))
                                {
                                    model.LoggerName = item.Replace("LoggerName:", "");
                                }
                                if (item.StartsWith("输出信息:"))
                                {
                                    model.Message = item.Replace("输出信息:", "");
                                }
                            }
                            list.Add(model);
                        }
                    }
                }
                return list;
            }
            catch (Exception e)
            {

                return null;
            }
        }
    }
    /// <summary>
    /// 日志信息  记录时间:%date▆线程ID:[%thread]▆日志级别:%-5level▆LoggerName:%logger▆输出信息:%message%newline
    /// </summary>
    public class LogInfo {
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 线程ID
        /// </summary>
        public int Thread { get; set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        public string Level { get; set; }
        /// <summary>
        /// LoggerName
        /// </summary>
        public string LoggerName { get; set; }
        /// <summary>
        /// 输出信息
        /// </summary>
        public string Message { get; set; } 

  

    }

    /// <summary>
    /// 日志等级
    /// </summary>
    public enum Log4level
    {
        /// <summary>
        /// 关闭
        /// </summary>
        OFF,
        /// <summary>
        /// 严重错误
        /// </summary>
        FATAL,
        /// <summary>
        /// 异常
        /// </summary>
        ERROR,
        /// <summary>
        /// 警告
        /// </summary>
        WARN,
        /// <summary>
        /// 信息
        /// </summary>
        INFO,
        /// <summary>
        /// 调试
        /// </summary>
        DEBUG,
        /// <summary>
        /// 所有
        /// </summary>
        ALL,
        /// <summary>
        /// 特殊输出方式不属于日志级别
        /// </summary>
        Console

    }
}
