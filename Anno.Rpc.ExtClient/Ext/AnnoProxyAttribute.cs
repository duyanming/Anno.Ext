/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/8/18 17:45:33 
Functional description： AnnoProxyAttribute
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Rpc.Client.Ext
{
    [AttributeUsage(AttributeTargets.Method| AttributeTargets.Interface,AllowMultiple =false,Inherited =false)]
    public class AnnoProxyAttribute: Attribute
    {
        public string Channel { get; set; }
        public string Router { get; set; }
        public string Method { get; set; }
    }
}
