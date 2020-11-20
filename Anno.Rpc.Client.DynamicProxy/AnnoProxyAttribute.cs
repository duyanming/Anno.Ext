using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Rpc.Client.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class AnnoProxyAttribute : Attribute
    {
        public string Channel { get; set; }
        public string Router { get; set; }
        public string Method { get; set; }
    }
}
