/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/8/18 18:02:49 
Functional description： ITaskService
******************************************************/
using Anno.EngineData;
using Anno.Rpc.Client.Ext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Test
{
    [AnnoProxy(Channel = "Anno.Plugs.Logic", Router = "Joke", Method = "Test0")]
    public interface ITaskService
    {
        [AnnoProxy(Channel = "", Method = "", Router = "")]
        ActionResult SayHi(string Name, TaskDto task);

        [AnnoProxy(Channel = "Anno.Plugs.Logic", Router = "CustomizeReturnType", Method = "SayHi")]
        string CustomizeSayHi(string name);
    }

    public class TaskDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
