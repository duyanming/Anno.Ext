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
    [AnnoProxy(Channel = "Anno.Plugs.Tita", Router = "Tita")]
    public interface ITitaService
    {
        /// <summary>
        /// 异步获取一个通知消息
        /// </summary>
        /// <param name="inputMsg"></param>
        /// <returns></returns>
        Task<NoticeMsg> GetNoticeMsgAsync(string inputMsg);
    }

    public class NoticeMsg
    {
        public string Message { get; set; }
    }
}
