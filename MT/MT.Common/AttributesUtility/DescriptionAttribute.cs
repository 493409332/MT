﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MT.Common.AttributesUtility
{
    /// <summary>
    /// 提供对类型、成员的描述功能
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class DescriptionAttribute : System.Attribute
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description">描述信息</param>
        public DescriptionAttribute(string description)
        {
            this.Description = description;
        }

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
