/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/8/18 18:01:41 
Functional description： AnnoRpcTest
******************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var taskService = AnnoProxyBuilder.GetService<ITaskService>();
            stopWatch.Stop();
            Console.WriteLine($"AnnoProxyBuilder.GetService：{stopWatch.Elapsed}");

            stopWatch.Restart();
            taskService = AnnoProxyBuilder.GetService<ITaskService>();
            stopWatch.Stop();
            Console.WriteLine($"AnnoProxyBuilder.GetService2：{stopWatch.Elapsed}");

            for (int i = 0; i < 1; i++)
            {
                var rlt1 = taskService.ServiceInstances();
                Console.WriteLine("ServiceInstances:" + Newtonsoft.Json.JsonConvert.SerializeObject(rlt1));
                Console.WriteLine("CustomizeSayHi:" + taskService.CustomizeSayHi("AnnoProxy"));
                Console.WriteLine("Add:" + taskService.Add(6,8));
                Console.WriteLine("Dyn:" + taskService.Dyn());
                Console.WriteLine("Object:" + taskService.Object());
                Console.WriteLine("Dynamic:" + taskService.Dynamic());
            }
        }
        static void Init()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            AnnoProxyBuilder.Init();
            stopWatch.Stop();
            Console.WriteLine($"AnnoProxyBuilder.Init(false)：{stopWatch.Elapsed}");
            DefaultConfigManager.SetDefaultConnectionPool(1000, Environment.ProcessorCount * 2, 50);
            DefaultConfigManager.SetDefaultConfiguration("RpcTest", "127.0.0.1", 7010, false);
        }
    }
}
