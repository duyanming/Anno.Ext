/****************************************************** 
Writer: Du YanMing-admin
Mail:dym880@163.com
Create Date: 8/23/2021 12:33:42 PM
Functional description： DTransactionModule
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;


namespace Anno.Plugs.DTransactionService
{
    using Anno.EngineData;
    public class DTransactionModule : BaseModule
    {
        /// <summary>
        /// 开始Saga
        /// </summary>
        /// <param name="globalTraceId"></param>
        /// <returns></returns>
        public dynamic SagaStarted(string globalTraceId)
        {

            return null;
        }
        /// <summary>
        /// 记录子任务
        /// </summary>
        /// <param name="globalTraceId"></param>
        /// <param name="traceId"></param>
        /// <returns></returns>
        public dynamic SagaSub(string globalTraceId, string traceId, string recover)
        {

            return null;
        }
        /// <summary>
        /// 恢复已成功的任务
        /// </summary>
        /// <param name="globalTraceId"></param>
        /// <returns></returns>
        public dynamic SagaRecovery(string globalTraceId)
        {

            return null;
        }
        /// <summary>
        /// （全部成功）结束分布式任务
        /// </summary>
        /// <param name="globalTraceId"></param>
        /// <returns></returns>
        public dynamic SagaEnd(string globalTraceId)
        {

            return null;
        }
    }
}
