using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using MT.Common.ConfigHelper;
using System.Collections.Generic;
using MT.AOP.Factory;
using Complex.Logical.Admin;
using MT.Redis;

namespace MT.UnitTest
{
 
    [TestClass]
    public class UnitTest_MT_Redis
    {
       
        [TestMethod]
        public void TestRedis()
        {
            RedisHelper redis = new RedisHelper("192.168.10.5:7000,abortConnect=false");
            string name= redis.Get<string>("name");
            Assert.AreEqual(name,"miantiao");

        }
    }

}
