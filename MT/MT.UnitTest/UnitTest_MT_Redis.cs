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

            RedisHelper redis = new RedisHelper();

            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            sw.Restart();
            string name = redis.Get<string>("name");

            sw.Stop();
            ts2 = sw.Elapsed;
            Assert.AreEqual(name, "miantiao");
            var tran = redis.CreateTransaction();

            tran.StringSetAsync("pwd1", "123123", TimeSpan.FromSeconds(6));

           // RedisValue pwd =   tran.StringGetAsync((RedisKey)"pwd1").Result;

            bool committed = tran.Execute();

            string pwd1 = redis.Get<string>("pwd1");
            var tpwd = GetpwdAsync();
  
            List<KeyValuePair<RedisKey, RedisValue>> redislist = new List<KeyValuePair<RedisKey, RedisValue>>()
            {  new KeyValuePair<RedisKey, RedisValue>("mykey", "myvalue"),
                new KeyValuePair<RedisKey, RedisValue>("mykey1", "myvalue1")
            };

            //var qq = redis.HashKeys<string>("*");

            //    List<string> list = redis.Get<string>(new List<string>(){"pwd","name"});
 
                    //       redis.Set(redislist );

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
