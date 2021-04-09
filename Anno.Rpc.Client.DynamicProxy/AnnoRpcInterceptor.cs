using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Anno.Rpc.Client.DynamicProxy
{
    using Anno.Rpc.Client;
    public class AnnoRpcInterceptor : IInterceptor
    {
        private static Type _taskType = typeof(Task);
        public void Intercept(IInvocation invocation)
        {
            Dictionary<string, string> input = new Dictionary<string, string>();
            AnnoProxyAttribute proxyAttribute = new AnnoProxyAttribute();
            var moduleAttribute = invocation.Method.DeclaringType.GetCustomAttributes<AnnoProxyAttribute>().FirstOrDefault();
            var methodAttribute = invocation.Method.GetCustomAttributes<AnnoProxyAttribute>().FirstOrDefault();

            if (moduleAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(moduleAttribute.Channel))
                {
                    proxyAttribute.Channel = moduleAttribute.Channel;
                }
                if (!string.IsNullOrWhiteSpace(moduleAttribute.Router))
                {
                    proxyAttribute.Router = moduleAttribute.Router;
                }
                if (!string.IsNullOrWhiteSpace(moduleAttribute.Method))
                {
                    proxyAttribute.Method = moduleAttribute.Method;
                }
            }
            if (methodAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(methodAttribute.Channel))
                {
                    proxyAttribute.Channel = methodAttribute.Channel;
                }
                if (!string.IsNullOrWhiteSpace(methodAttribute.Router))
                {
                    proxyAttribute.Router = methodAttribute.Router;
                }
                if (!string.IsNullOrWhiteSpace(methodAttribute.Method))
                {
                    proxyAttribute.Method = methodAttribute.Method;
                }
            }
            if (string.IsNullOrWhiteSpace(proxyAttribute.Channel))
            {
                proxyAttribute.Method = invocation.Method.DeclaringType.Module.Name;
            }
            if (string.IsNullOrWhiteSpace(proxyAttribute.Router))
            {
                proxyAttribute.Method = invocation.Method.DeclaringType.Name;
            }
            if (string.IsNullOrWhiteSpace(proxyAttribute.Method))
            {
                proxyAttribute.Method = invocation.Method.Name;
            }
            input.AddOrUpdate("channel", proxyAttribute.Channel);
            input.AddOrUpdate("router", proxyAttribute.Router);
            input.AddOrUpdate("method", proxyAttribute.Method);
            var _params = invocation.Method.GetParameters();
            for (int i = 0; i < _params.Length; i++)
            {
                var param = _params[i];
                if (param.ParameterType.IsClass && param.ParameterType != "".GetType())
                {
                    input.AddOrUpdate(param.Name, JsonConvert.SerializeObject(invocation.Arguments[i]));
                }
                else
                {
                    input.AddOrUpdate(param.Name, invocation.Arguments[i].ToString());
                }
            }
            if (invocation.Method.ReturnType != typeof(void))
            {
                var rltStr = Connector.BrokerDns(input);
                Type realReturnType = null;
                bool isTask = false;
                if (invocation.Method.ReturnType.BaseType != null && invocation.Method.ReturnType.BaseType.FullName.Equals("System.Threading.Tasks.Task"))
                {
                    var generics = invocation.Method.ReturnType.GenericTypeArguments;
                    if (generics != null && generics.Length > 0)
                    {
                        realReturnType = generics[0];
                    }
                    else
                    {
                        realReturnType = _taskType;
                    }
                    isTask = true;
                }
                else
                {
                    realReturnType = invocation.Method.ReturnType;
                }

                if (realReturnType == _taskType)
                {
                    invocation.ReturnValue = Task.CompletedTask;
                }
                else
                {
                    dynamic returnValue;
                    if (typeof(EngineData.IActionResult).IsAssignableFrom(realReturnType))
                    {
                        returnValue = JsonConvert.DeserializeObject(rltStr, type: realReturnType);
                    }
                    else
                    {
                        var rltType = Type.GetType("Anno.EngineData.ActionResult`1, Anno.EngineData").MakeGenericType(realReturnType);
                        var data = JsonConvert.DeserializeObject(rltStr, rltType) as dynamic;
                        if (data.Status == false && data.OutputData == null && !string.IsNullOrWhiteSpace(data.Msg))
                        {
                            throw new AnnoRpcException(data.Msg);
                        }
                        else
                        {
                            returnValue = data.OutputData;
                        }
                    }

                    invocation.ReturnValue = isTask ? Task.FromResult(returnValue) : returnValue;
                }
            }
            else
            {
                Connector.BrokerDns(input);
            }
        }
    }
}
