using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using MT.AOP.Context;
using MT.AOP.Attributes;

namespace Complex.Logical
{

    public class BaseEndAttribute : PostAspectAttribute
    {
        public override InvokeContext Action(InvokeContext context)
        {
            Console.WriteLine("log end!");
            return context;
        }
    }
}
