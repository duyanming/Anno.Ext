/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/8/18 18:02:49 
Functional description： ITaskService
******************************************************/
using Anno.EngineData;
using Anno.Rpc.Client.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Test
{
    [AnnoProxy(Channel = "Anno.Plugs.HelloWorld", Router = "HelloWorldViper")]
    public interface IHelloWorldService
    {
        dynamic SayHello(string name, int age);
    }
}
