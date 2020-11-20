using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Rpc.Client.DynamicProxy
{
    public class AnnoRpcException : Exception
    {
        public AnnoRpcException() : base()
        {

        }

        public AnnoRpcException(string msg) : base(msg)
        {
        }
    }
}
