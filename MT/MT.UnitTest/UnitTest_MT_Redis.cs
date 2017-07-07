using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using MT.Common.ConfigUtility;
using System.Collections.Generic;
using MT.AOP.Factory;
using Complex.Logical.Admin;
using MT.Redis;
using System.Diagnostics;

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
            string name= redis.Get<string>("name");

            sw.Stop();
              ts2 = sw.Elapsed;
            Assert.AreEqual(name,"miantiao");

        }
    }

}
