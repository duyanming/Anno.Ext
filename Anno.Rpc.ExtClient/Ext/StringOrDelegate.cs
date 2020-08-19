/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/8/18 16:58:35 
Functional description： StringOrDelegate
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Rpc.Client.Ext
{
    public class StringOrDelegate
    {
        public readonly Delegate DelegateParameter;
        public readonly string StringParameter;

        public StringOrDelegate(string value, Delegate @delegate = default)
        {
            DelegateParameter = @delegate;
            StringParameter = value;
        }
        public StringOrDelegate(Delegate @delegate, string value = default)
        {
            DelegateParameter = @delegate;
            StringParameter = value;
        }

        public static implicit operator StringOrDelegate(string value)
        {
            return new StringOrDelegate(value);
        }
        public static implicit operator StringOrDelegate(Delegate value)
        {
            return new StringOrDelegate(value);
        }
    }
}
