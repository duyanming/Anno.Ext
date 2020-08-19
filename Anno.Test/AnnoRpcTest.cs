/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/8/18 18:01:41 
Functional description： AnnoRpcTest
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using Anno.Rpc.Client;
using Anno.Rpc.Client.Ext;

namespace Anno.Test
{
    public class AnnoRpcTest
    {
        public static void Handle()
        {
            Init();
            for (int i = 0; i < 100; i++)
            {
                var taskService = AnnoProxyBuilder.GetService<ITaskService>();
                var taskServiceType = AnnoProxyBuilder.GetServiceType<ITaskService>();
                var rlt = taskService.SayHi("Du Yan Ming", new TaskDto() { Name = "Anno", Age = 18 });

                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(rlt));
            }
        }
        static void Init()
        {
            AnnoProxyBuilder.Init();
            DefaultConfigManager.SetDefaultConnectionPool(1000, Environment.ProcessorCount * 2, 50);
            DefaultConfigManager.SetDefaultConfiguration("RpcTest", "127.0.0.1", 6660, false);
        }
    }
}
