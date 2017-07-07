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
           
        

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
               
                Log4Helper.ConsoleInfo("呵呵呵 start!");
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();

        }
    }
}