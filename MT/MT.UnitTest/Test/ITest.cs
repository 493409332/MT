

using System;

namespace Complex.Logical.Admin
{

    public interface ITest
    {
        object test(int a, int b, string c, object d, ref int e, out int f, params int[] A);
    }
    public class MyTest : ITest
    {


        [BaseStart, BaseEnd, BaseEx]
        public object test(int a, int b, string c, object d, ref int e, out int f, params int[] A)
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
