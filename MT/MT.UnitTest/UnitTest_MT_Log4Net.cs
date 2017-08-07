using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using MT.Common.ConfigUtility;
using System.Collections.Generic;
using MT.AOP.Factory;
using Complex.Logical.Admin;
using MT.Redis;
using System.Diagnostics;
using MT.Common.Log4Utility;
using System.IO;

namespace MT.UnitTest
{
 
    [TestClass]
    public class UnitTest_MT_Log4Net
    {

        [TestMethod]
        public void TestLog4()
        {
            var quer = Log4Helper.ReadLogList(string.Format( "logfile/{0}.log",DateTime.Now.ToString("yyyyMMdd")));
         
            int logCount = quer.Count;
            Log4Helper.Info("te st");

            Log4Helper.Fatal("Fatal");
            Log4Helper.Error("Error");
            quer = Log4Helper.ReadLogList(string.Format("logfile/{0}.log", DateTime.Now.ToString("yyyyMMdd")));

            Assert.IsTrue((logCount + 3) == quer.Count);
        }
    }

}
