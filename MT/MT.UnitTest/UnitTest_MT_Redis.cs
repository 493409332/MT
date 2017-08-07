using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using MT.Common.ConfigUtility;
using System.Collections.Generic;
using MT.AOP.Factory;
using Complex.Logical.Admin;
using MT.Redis;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using StackExchange.Redis;

namespace MT.UnitTest
{

    [TestClass]
    public class UnitTest_MT_Redis
    {

        [TestMethod]
        public void TestRedis()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            RedisHelper redis =   RedisHelper.Instance;

            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            sw.Restart();
            string name = redis.Get<string>("name");

            sw.Stop();
            ts2 = sw.Elapsed;
            Assert.AreEqual(name, "miantiao");


            var tran = redis.CreateTransaction();

            var pwd1stata= tran.StringSetAsync((RedisKey)"pwd1", (RedisValue)"123123", TimeSpan.FromSeconds(6));
             
            //Assert.AreEqual(pwd1stata.Result, true);
 
            bool committed = tran.Execute();
            
            string pwd1 = redis.Get<string>("pwd1");




            var tpwd = GetpwdAsync();
  
   

            Assert.AreEqual(tpwd.Result, "123123");

    

        }

        async static Task<string> GetpwdAsync()
        {

            return await Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(4));
                return RedisHelper.Instance.Get<string>("pwd1");
            });

        }
    }

}
