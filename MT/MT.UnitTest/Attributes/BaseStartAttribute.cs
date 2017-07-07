using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MT.AOP.Attributes;
using MT.AOP.Context;
using System.Runtime.InteropServices;
using System.Reflection;
using MT.Common.Log4Utility;

namespace Complex.Logical
{
    public class BaseStartAttribute : PreAspectAttribute
    {
        public override InvokeContext Action(InvokeContext context)
        { 
            Log4Helper.GetLog(Log4level.Console).Info("1111log start!");

            context.ParametersFull["a"] = 100; 
            context.ParametersFull["b"] = 200;  
            context.ParametersFull["c"] = "哈哈哈";
            context.ParametersFull["d"] = "哈哈哈";
            context.ParametersFull["A"] = new int[]{10,20,30,40};

            //Type[] types = new Type[]
            //{ 
            //    typeof(int).MakeByRefType() 
            //};


            //MethodInfo method = GetType().GetMethod("RefHe", types);
            //method.Invoke(this,new object[] {  context.ParametersFull["e"] });
            // RefHe(  context.ParametersFull["e"]);

            //IntPtr ptr = GCHandle.ToIntPtr(handle);

            //Marshal.WriteInt32(ptr, 100);
            context.IsRun = true;
            return context;
        }

        public void RefHe(ref int a) {

            a = 111;
        }
    }
}
