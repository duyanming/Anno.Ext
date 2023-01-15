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
    using System.Numerics;

    public class AnnoRpcInterceptor : IInterceptor
    {
        private static Type _taskType = typeof(Task);
        private static Type rltObjectType = typeof(Anno.EngineData.ActionResult<>);
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
                //null 不传递
                if (invocation.Arguments[i] == null)
                {
                    continue;
                }
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
                if (rltStr.IndexOf("tatus\":false") > 0)
                {
                    string errorMsg = "服务端未知错误";
                    try
                    {
                        var errorData = JsonConvert.DeserializeObject(rltStr, rltObjectType) as dynamic;
                        errorMsg = errorData.Msg;
                    }
                    catch
                    {
                        int errorMsgLength = rltStr.IndexOf("\",\"Status\"");
                        if (errorMsgLength > 11)
                        {
                            errorMsg = rltStr.Substring(8, errorMsgLength - 8);
                        }
                    }
                    throw new AnnoRpcException(errorMsg);
                }

                bool isTask = false;
                Type realReturnType;
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
                    invocation.ReturnValue = Task.Factory.StartNew(() => { });
                }
                else
                {
                    dynamic returnValue;
                    if (typeof(Anno.EngineData.IActionResult).IsAssignableFrom(realReturnType))
                    {
                        returnValue = JsonConvert.DeserializeObject(rltStr, type: realReturnType);
                    }
                    else
                    {
                        var rltType = typeof(Anno.EngineData.ActionResult<>).MakeGenericType(realReturnType);
                        var data = JsonConvert.DeserializeObject(rltStr, rltType) as dynamic;
                        if (data.Status == false && data.OutputData == null && !string.IsNullOrWhiteSpace(data.Msg))
                        {
                            throw new AnnoRpcException(data.Msg);
                        }
                        else
                        {
                            returnValue = data.OutputData;
                        }

                        if (returnValue == null && isTask)
                        {
                            if (realReturnType == typeof(object)) { invocation.ReturnValue = Task.FromResult<object>(default); return; }

                            if (realReturnType == typeof(string)) { invocation.ReturnValue = Task.FromResult<string>(default); return; }
                            if (realReturnType == typeof(char[])) { invocation.ReturnValue = Task.FromResult<char[]>(default); return; }
                            if (realReturnType == typeof(char)) { invocation.ReturnValue = Task.FromResult<char>(default); return; }
                            if (realReturnType == typeof(bool)) { invocation.ReturnValue = Task.FromResult<bool>(default); return; }
                            if (realReturnType == typeof(bool?)) { invocation.ReturnValue = Task.FromResult<bool?>(default); return; }
                            if (realReturnType == typeof(byte)) { invocation.ReturnValue = Task.FromResult<byte>(default); return; }
                            if (realReturnType == typeof(byte?)) { invocation.ReturnValue = Task.FromResult<byte?>(default); return; }
                            if (realReturnType == typeof(decimal)) { invocation.ReturnValue = Task.FromResult<decimal>(default); return; }
                            if (realReturnType == typeof(decimal?)) { invocation.ReturnValue = Task.FromResult<decimal?>(default); return; }
                            if (realReturnType == typeof(double)) { invocation.ReturnValue = Task.FromResult<double>(default); return; }
                            if (realReturnType == typeof(double?)) { invocation.ReturnValue = Task.FromResult<double?>(default); return; }
                            if (realReturnType == typeof(float)) { invocation.ReturnValue = Task.FromResult<float>(default); return; }
                            if (realReturnType == typeof(float?)) { invocation.ReturnValue = Task.FromResult<float?>(default); return; }
                            if (realReturnType == typeof(int)) { invocation.ReturnValue = Task.FromResult<int>(default); return; }
                            if (realReturnType == typeof(int?)) { invocation.ReturnValue = Task.FromResult<int?>(default); return; }
                            if (realReturnType == typeof(long)) { invocation.ReturnValue = Task.FromResult<long>(default); return; }
                            if (realReturnType == typeof(long?)) { invocation.ReturnValue = Task.FromResult<long?>(default); return; }
                            if (realReturnType == typeof(sbyte)) { invocation.ReturnValue = Task.FromResult<sbyte>(default); return; }
                            if (realReturnType == typeof(sbyte?)) { invocation.ReturnValue = Task.FromResult<sbyte?>(default); return; }
                            if (realReturnType == typeof(short)) { invocation.ReturnValue = Task.FromResult<short>(default); return; }
                            if (realReturnType == typeof(short?)) { invocation.ReturnValue = Task.FromResult<short?>(default); return; }
                            if (realReturnType == typeof(uint)) { invocation.ReturnValue = Task.FromResult<uint>(default); return; }
                            if (realReturnType == typeof(uint?)) { invocation.ReturnValue = Task.FromResult<uint?>(default); return; }
                            if (realReturnType == typeof(ulong)) { invocation.ReturnValue = Task.FromResult<ulong>(default); return; }
                            if (realReturnType == typeof(ulong?)) { invocation.ReturnValue = Task.FromResult<ulong?>(default); return; }
                            if (realReturnType == typeof(ushort)) { invocation.ReturnValue = Task.FromResult<ushort>(default); return; }
                            if (realReturnType == typeof(ushort?)) { invocation.ReturnValue = Task.FromResult<ushort?>(default); return; }
                            if (realReturnType == typeof(DateTime)) { invocation.ReturnValue = Task.FromResult<DateTime>(default); return; }
                            if (realReturnType == typeof(DateTime?)) { invocation.ReturnValue = Task.FromResult<DateTime?>(default); return; }
                            if (realReturnType == typeof(DateTimeOffset)) { invocation.ReturnValue = Task.FromResult<DateTimeOffset>(default); return; }
                            if (realReturnType == typeof(DateTimeOffset?)) { invocation.ReturnValue = Task.FromResult<DateTimeOffset?>(default); return; }
                            if (realReturnType == typeof(TimeSpan)) { invocation.ReturnValue = Task.FromResult<TimeSpan>(default); return; }
                            if (realReturnType == typeof(TimeSpan?)) { invocation.ReturnValue = Task.FromResult<TimeSpan?>(default); return; }
                            if (realReturnType == typeof(Guid)) { invocation.ReturnValue = Task.FromResult<Guid>(default); return; }
                            if (realReturnType == typeof(Guid?)) { invocation.ReturnValue = Task.FromResult<Guid?>(default); return; }
                            if (realReturnType == typeof(BigInteger)) { invocation.ReturnValue = Task.FromResult<BigInteger>(default); return; }
                            if (realReturnType == typeof(BigInteger?)) { invocation.ReturnValue = Task.FromResult<BigInteger?>(default); return; }

                            returnValue = JsonConvert.DeserializeObject("{}", type: realReturnType);
                            var returnValueTask = Task.FromResult(returnValue);
                            invocation.ReturnValue = returnValueTask;
                            return;
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
