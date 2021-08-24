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
        /// <param name="globalTraceId"></param>
        /// <param name="recover"></param>
        /// <returns></returns>
        public dynamic SagaStarted(string globalTraceId, string recover, Dictionary<string, string> sagaInput)
        {
            DTransactionManager.Sagas.TryGetValue(globalTraceId, out ConcurrentStack<SagaTx> txs);
            if (txs == null)
            {
                txs = new ConcurrentStack<SagaTx>();
                txs.Push(new SagaTx()
                {
                    sagaGlobalId = globalTraceId,
                    recover = recover,
                    sagaInput = sagaInput,
                    sagaId = globalTraceId
                });
                DTransactionManager.Sagas.TryAdd(globalTraceId, txs);
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

            DTransactionManager.Sagas.TryGetValue(globalTraceId, out ConcurrentStack<SagaTx> txs);
            if (txs != null && !txs.ToList().Exists(tx => tx.sagaId == traceId))
            {
                txs.Push(new SagaTx()
                {
                    sagaGlobalId = globalTraceId,
                    recover = recover,
                    sagaInput = sagaInput,
                    sagaId = traceId,
                    sagaRlt = sagaRlt
                });
                DTransactionManager.Sagas.TryAdd(globalTraceId, txs);
            }
            return true;
        }
        /// <summary>
        /// 恢复已成功的任务
        /// </summary>
        /// <param name="globalTraceId"></param>
        /// <returns></returns>
        public dynamic SagaRecovery(string globalTraceId)
        {
            DTransactionManager.Sagas.TryRemove(globalTraceId, out ConcurrentStack<SagaTx> txs);
            while (txs.TryPop(out SagaTx tx))
            {
                try
                {
                    var response = tx.sagaInput;
                    string channel, router, method;
                    channel = tx.sagaInput[Eng.NAMESPACE];
                    router = tx.sagaInput[Eng.CLASS];
                    method = tx.recover;
                    response.Add("sagaRlt", tx.sagaRlt);

                    if (response.ContainsKey("TraceId")) {
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
            return true;
        }
        /// <summary>
        /// （全部成功）结束分布式任务
        /// </summary>
        /// <param name="globalTraceId"></param>
        /// <returns></returns>
        public dynamic SagaEnd(string globalTraceId)
        {
            DTransactionManager.Sagas.TryRemove(globalTraceId, out ConcurrentStack<SagaTx> txs);
            return true;
        }
    }
}
