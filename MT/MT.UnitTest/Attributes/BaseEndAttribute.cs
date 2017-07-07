using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using MT.AOP.Context;
using MT.AOP.Attributes;
using MT.Common.Log4Utility;

namespace Complex.Logical
{

    public class BaseEndAttribute : PostAspectAttribute
    {
        public override InvokeContext Action(InvokeContext context)
        {
            Log4Helper.GetLog(Log4level.Console).Info("1111log end!");
          
            return context;
        }
    }
}
