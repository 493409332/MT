using MT.Common.Log4Utility;
using System;
using System.Text;
 
namespace MT.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Encoding.
           
            Log4Helper.GetLog(Log4level.Console).Info("呵呵呵 start!");

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.WriteLine(Encoding.GetEncoding("GB2312"));
                Console.WriteLine("您好，北京欢迎你");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();

        }
    }
}