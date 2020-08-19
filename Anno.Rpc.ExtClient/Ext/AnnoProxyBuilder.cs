﻿/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/8/18 17:11:41 
Functional description： AnnoProxyBuilder
******************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Anno.Rpc.Client.Ext
{
    public class AnnoProxyBuilder
    {
        private static bool isInit = false;
        private static ConcurrentDictionary<string, object> caches = new ConcurrentDictionary<string, object>();

        public static TService GetService<TService>()
        {
            if (!isInit)
            {
                NatashaInitializer.InitializeAndPreheating().Wait();
                isInit = true;
            }
            var iType = typeof(TService).FullName;
            if (caches.TryGetValue(iType, out object service))
            {
                return (TService)service;
            }
            var instance = GetInstance<TService>();
            caches.TryAdd(iType, instance);
            return instance;
        }

        public static Type GetServiceType<TService>()
        {
            if (!isInit)
            {
                NatashaInitializer.InitializeAndPreheating().Wait();
                isInit = true;
            }
            var iType = typeof(TService).FullName;
            if (caches.TryGetValue(iType, out object service))
            {
                return ((TService)service).GetType();
            }
            service = GetInstance<TService>();
            caches.TryAdd(iType, service);
            return ((TService)service).GetType();
        }
        /// <summary>
        /// 获取代理实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        private static TService GetInstance<TService>()
        {
            var proxy = new AnnoProxy<TService>().UseSingleton(true);
            foreach (var method in proxy.NeedReWriteMethods.ToArray())
            {
                var methodInfo = proxy.TryGetMethod(method.Key);
                if (methodInfo == null)
                {
                    continue;
                }
                proxy[method.Key] = BuildScript(typeof(TService), methodInfo);
            }
            var taskServiceImp = proxy.GetCreator<TService>();
            return taskServiceImp();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (!isInit)
            {
                NatashaInitializer.InitializeAndPreheating().Wait();
                isInit = true;
            }
        }
        private static string BuildScript(Type service, System.Reflection.MethodInfo methodInfo)
        {
            StringBuilder script = new StringBuilder();
            script.AppendLine(@"Dictionary<string, string> input = new Dictionary<string, string>();");
            script.AppendLine("input.Add(\"channel\", \"\");");
            script.AppendLine("input.Add(\"router\", \"\");");
            script.AppendLine("input.Add(\"method\", \"\");");
            var attributes = service.GetCustomAttributes(typeof(AnnoProxyAttribute), true);
            var methodAttributes = methodInfo.GetCustomAttributes(typeof(AnnoProxyAttribute), true);
            /*
             * 接口模块 通道设置
             */
            if (attributes.Length > 0 || methodAttributes.Length > 0)
            {
                AnnoProxyAttribute attribute = null;
                if (attributes.Length > 0)
                {
                    attribute = (AnnoProxyAttribute)attributes[0];
                }
                if (attribute == null && methodAttributes.Length > 0)
                {
                    attribute = (AnnoProxyAttribute)methodAttributes[0];
                }
                else if (attribute != null && methodAttributes.Length > 0)
                {
                    AnnoProxyAttribute mAttribute = (AnnoProxyAttribute)methodAttributes[0];
                    if (!string.IsNullOrWhiteSpace(mAttribute.Channel))
                    {
                        attribute.Channel = mAttribute.Channel;
                    }
                    if (!string.IsNullOrWhiteSpace(mAttribute.Router))
                    {
                        attribute.Router = mAttribute.Router;
                    }
                    if (!string.IsNullOrWhiteSpace(mAttribute.Method))
                    {
                        attribute.Method = mAttribute.Method;
                    }
                }
                if (!string.IsNullOrWhiteSpace(attribute.Channel))
                {
                    script.AppendLine($"input[\"channel\"] = \"{attribute.Channel}\";");

                }
                if (!string.IsNullOrWhiteSpace(attribute.Router))
                {
                    script.AppendLine($"input[\"router\"] = \"{attribute.Router}\";");
                }
                if (!string.IsNullOrWhiteSpace(attribute.Method))
                {
                    script.AppendLine($"input[\"method\"] = \"{attribute.Method}\";");
                }
            }
            else
            {
                throw new ArgumentNullException("请设置管道参数 AnnoProxyAttribute");
            }
            var parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.IsClass && !parameters[i].ParameterType.Equals("".GetType()))
                {
                    script.AppendLine($"input.Add(\"{parameters[i].Name}\", Newtonsoft.Json.JsonConvert.SerializeObject({parameters[i].Name}));");
                }
                else
                {
                    script.AppendLine($"input.Add(\"{parameters[i].Name}\", {parameters[i].Name}.ToString());");
                }
            }
            if (methodInfo.ReturnType != typeof(void))
            {
                script.AppendLine("string rlt = Anno.Rpc.Client.Connector.BrokerDns(input);");
                if (methodInfo.ReturnType.IsClass && !methodInfo.ReturnType.Equals("".GetType()))
                {
                    script.AppendLine($"return Newtonsoft.Json.JsonConvert.DeserializeObject<{methodInfo.ReturnType.FullName}>(rlt);");
                }
                else
                {
                    throw new ArgumentNullException("返回类型必须是 void 或者 Anno.EngineData.ActionResult");
                }
            }
            else
            {

                script.AppendLine(" string rlt = Anno.Rpc.Client.Connector.BrokerDns(input);");
            }
            return script.ToString();
        }
    }
}
