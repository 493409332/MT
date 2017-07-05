using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using MT.Common.ConfigHelper;
using System.Collections.Generic;
using MT.AOP.Factory;
using Complex.Logical.Admin;
using MT.Redis;
using System.Diagnostics;

namespace MT.UnitTest
{
 
    [TestClass]
    public class UnitTest_MT_Log4Net
    {
       
        [TestMethod]
        public void TestLog4()
        {
            Log4Net.Logtest();
        }
    }

}
