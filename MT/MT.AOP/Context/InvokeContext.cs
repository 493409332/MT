﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace MT.AOP.Context
{

    public class DictionaryS<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public List<object> _parameters;
        public new TValue this[TKey key]
        {

            get
            {
                IDictionaryEnumerator _hashEnum = this.GetEnumerator();
                while (_hashEnum.MoveNext())
                {
                    if (_hashEnum.Key == (object)key)
                    {
                        return (TValue)_hashEnum.Value;
                    }

                }
                return default(TValue);
            }

            set
            {
                this.Remove(key);
                this.Add(key, value);
                _parameters.Clear();
                IDictionaryEnumerator _hashEnum = this.GetEnumerator();
                while (_hashEnum.MoveNext())
                {
                    _parameters.Add(_hashEnum.Value);

                }

            }
        }

    }
    /// <summary>
    /// 所有元数据
    /// </summary>
    public class InvokeContext
    {

        //List<ParameterMetadata> _parameters;
        List<object> _parameters = new List<object>();
        DictionaryS<string, object> _parametersfull = new DictionaryS<string, object>();

        bool _IsRun = true;
        /// <summary>
        /// 是否继续运行
        /// </summary>
        public bool IsRun
        {
            get { return _IsRun; }
            set { _IsRun = value; }
        }


        public List<object> Parameters
        {
            get
            {
                if (_parameters != null)
                {
                    return _parameters;
                }

                return null;
            }
        }

        public DictionaryS<string, object> ParametersFull
        {
            get
            {
                if (_parametersfull != null)
                {
                    return _parametersfull;
                }

                return null;
            }
        }



        string _method;
        /// <summary>
        /// 方法元数据
        /// </summary>
        public string MethodName
        {
            get
            {

                return _method;
            }
        }
        string _className;
        /// <summary>
        /// 类全名元数据
        /// </summary>
        public string ClassFullName
        {
            get
            {

                return _className;
            }
        }
        object _result;
        /// <summary>
        /// 返回元数据
        /// </summary>
        public object Result
        {
            set
            {
                _result = value;
            }
            get
            {
                return _result;
            }
        }

        Exception _ex;
        /// <summary>
        /// 异常元数据
        /// </summary>
        public Exception Ex
        {
            get
            {
                return _ex;
            }
        }
        /// <summary>
        /// 返回值类型
        /// </summary>
        public Type ResultType { get; set; }

        /// <summary>
        /// 添加参数元数据
        /// </summary>
        /// <param name="parameter"></param>
        public void SetParameter(object parameter)
        {
            _parameters.Add(parameter);
        }


        public void SetParameterFull(string name, object parameter)
        {
            _parametersfull.Add(name, parameter);
            SetParameter(parameter);
            _parametersfull._parameters = _parameters;

        }
        /// <summary>
        /// 添加异常元数据
        /// </summary>
        /// <param name="e"></param>
        public void SetError(Exception e)
        {
            _ex = e;
        }
        /// <summary>
        /// 添加返回元数据
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(object result)
        {
            _result = result;
        }
        /// <summary>
        /// 添加方法元数据
        /// </summary>
        /// <param name="methodName"></param>
        public void SetMethod(string methodName)
        {
            _method = methodName;
        }
        /// <summary>
        /// 添加方法元数据
        /// </summary>
        /// <param name="methodName"></param>
        public void SetClassName(string className)
        {
            _className = className;
        }



    }
}
