using System;
using System.Collections.Generic;
using System.Text;

namespace MT.IOC.Attribute
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class IOCConfigAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="baseType"></param>
        public IOCConfigAttribute(string description = "")
        {
            this.Description = description;
            //  TransactionEnable = transactionEnable;
        }

        ///// <summary>
        ///// 父类类型 开启需要继承 EFRepositoryBase
        ///// </summary>
        //public bool TransactionEnable { get; set; }



        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}
