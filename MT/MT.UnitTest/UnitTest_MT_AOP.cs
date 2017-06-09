using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using MT.Common.ConfigHelper;
using System.Collections.Generic;
using MT.AOP.Factory;
using Complex.Logical.Admin;

namespace MT.UnitTest
{
 
    [TestClass]
    public class UnitTest_MT_AOP
    {
       
        [TestMethod]
        public void TestProxyFactory()
        {
            ITest test= ProxyFactory.CreateProxy<ITest>(typeof(MyTest));
            int a = 1;
            int b;
            object dy = test.test(3, 1, "He", "He", ref a, out b, 1, 2, 3);
        }
    }

}
