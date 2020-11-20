using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Rpc.Client.DynamicProxy
{
    public static class AnnoRpcInterceptorExtension
    {

        public static void AddOrUpdate(this Dictionary<string, string> dic, string key, string value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }
    }
}
