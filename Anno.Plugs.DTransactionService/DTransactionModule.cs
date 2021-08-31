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
    using Anno.Const.Enum;
    using System.Collections.Concurrent;
    using System.Linq;

    public class DTransactionModule : BaseModule
    {
        /// <summary>
        /// 开始Saga
        /// </summary>
        /// <param name="globalTraceId">全局追踪ID</param>
        /// <param name="recover">恢复方法</param>
        /// <param name="sagaInput">输入</param>
        /// <param name="executionTimeSpan">预估整个任务执行时间长度</param>
        /// <returns></returns>
        public dynamic SagaStarted(string globalTraceId, string recover, Dictionary<string, string> sagaInput,double executionTimeSpan)
        {
            DTransactionManager.Sagas.TryGetValue(globalTraceId, out SagaTxs sagaTxs);
            if (sagaTxs == null)
            {
                sagaTxs = new SagaTxs();
                sagaTxs.Sagas.Push(new SagaTx()
                {
                    sagaGlobalId = globalTraceId,
                    recover = recover,
                    sagaInput = sagaInput,
                    sagaId = globalTraceId
                });
                sagaTxs.Deadline=DateTime.Now.AddMilliseconds(executionTimeSpan);
                DTransactionManager.Sagas.TryAdd(globalTraceId, sagaTxs);
            }
            return true;
        }
        /// <summary>
        /// 记录子任务
        /// </summary>
        /// <param name="globalTraceId"></param>
        /// <param name="traceId"></param>
        /// <param name="recover"></param>
        /// <param name="sagaInput"></param>
        /// <returns></returns>
        public dynamic SagaSub(string globalTraceId, string traceId, string recover, Dictionary<string, string> sagaInput, string sagaRlt)
        {
            if (string.IsNullOrWhiteSpace(globalTraceId)) {
                return false;
            }
            DTransactionManager.Sagas.TryGetValue(globalTraceId, out SagaTxs sagaTxs);
            if (sagaTxs != null && !sagaTxs.Sagas.ToList().Exists(tx => tx.sagaId == traceId))
            {
                sagaTxs.Sagas.Push(new SagaTx()
                {
                    sagaGlobalId = globalTraceId,
                    recover = recover,
                    sagaInput = sagaInput,
                    sagaId = traceId,
                    sagaRlt = sagaRlt
                });
            }
            return true;
        }
        /// <summary>
        /// 恢复已成功的任务
        /// </summary>
        /// <param name="globalTraceId"></param>
        /// <returns></returns>
        public dynamic SagaSubError(string globalTraceId)
        {
            if (string.IsNullOrWhiteSpace(globalTraceId))
            {
                return false;
            }
            DTransactionManager.Sagas.TryGetValue(globalTraceId, out SagaTxs sagaTxs);
            sagaTxs.Success = false;
            return true;
        }
        /// <summary>
        /// （全部成功）结束分布式任务
        /// </summary>
        /// <param name="globalTraceId"></param>
        /// <returns></returns>
        public dynamic SagaEnd(string globalTraceId,bool sagaStartSuccess)
        {
            if (string.IsNullOrWhiteSpace(globalTraceId))
            {
                return false;
            }
            DTransactionManager.Sagas.TryRemove(globalTraceId, out SagaTxs sagaTxs);
            if (sagaTxs.Success == false|| sagaStartSuccess==false)
            {
                Recovery(sagaTxs);
            }
            return true;
        }
        private void Recovery(SagaTxs sagaTxs)
        {
            while (sagaTxs.Sagas.TryPop(out SagaTx tx))
            {
                try
                {
                    var response = tx.sagaInput;
                    string channel, router, method;
                    channel = tx.sagaInput[Eng.NAMESPACE];
                    router = tx.sagaInput[Eng.CLASS];
                    method = tx.recover;
                    response.Add("sagaRlt", tx.sagaRlt);

                    if (response.ContainsKey("TraceId"))
                    {
                        response.Remove("TraceId");
                    }

                    if (response.ContainsKey("PreTraceId"))
                    {
                        response.Remove("PreTraceId");
                    }
                    this.InvokeProcessor(channel, router, method, response);
#if DEBUG
                    Log.Log.WriteLine($"channel:{channel},router:{router},method:{method}       SagaRecovery!");
#endif
                }
                catch (Exception ex)
                {
                    Log.Log.Error(ex, typeof(DTransactionManager));
                }
            }
        }
    }
}
