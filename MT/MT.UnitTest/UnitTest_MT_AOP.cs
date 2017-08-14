using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using MT.Common.ConfigUtility;
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
            dynamic dy = test.test(3, 1, "He", "He", ref a, out b, 1, 2, 3);
            Assert.AreEqual(a, 100 + 200);
            Assert.AreEqual(b, 40+30+20 + 10);
            Assert.AreEqual(dy.a_b,100-200);
            Assert.AreEqual(dy.ab, 200+ 100);
            Assert.AreEqual(dy.axb,200*100);
            Assert.AreEqual(dy.cd, "哈哈哈" + "哈哈哈");



        
        }

        public object atest1(int a, int b, string c, object d, ref int e, out int f, params int[] A)
        {
            e = a + b;

            int sunm = 0;
            foreach (var item in A)
            {
                sunm += item;
            }
            f = sunm;
            return new { a_b = a - b, ab = a + b, axb = a * b, cd = c + d.ToString() };
        }
    }

}
