using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MT.AOP.Attributes;
using MT.AOP.Context;
using MT.Common.Log4Utility;

namespace Complex.Logical
{
    public class BaseExAttribute : ExceptionAspectAttribute
    {
        public override InvokeContext Action(InvokeContext context)
        {
     
            Log4Helper.GetLog(Log4level.Console).Info("log exception!");
            throw context.Ex;

        }
    }
}
