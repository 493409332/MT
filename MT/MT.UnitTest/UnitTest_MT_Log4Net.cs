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
            var quer = Log4Helper.ReadLogList("logfile/log-INFO.log");
            int logCount = quer.Count;
            Log4Helper.GetLog(Log4level.INFO).Info("te st");
            Log4Helper.GetLog(Log4level.INFO).Fatal("Fatal");

            quer = Log4Helper.ReadLogList("logfile/log-INFO.log");

            Assert.IsTrue((logCount + 2) == quer.Count);
        }
    }

}
