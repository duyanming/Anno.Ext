using Anno.EngineData.Filters;
using System;
using System.Collections.Generic;

namespace Anno.EngineData
{
    /// <summary>
    /// 子任务
    /// </summary>
    public class SagaSub : Attribute, IActionFilter, IExceptionFilter, IFilterMetadata
    {
        /// <summary>
        /// 失败后的补偿方法（必须在当前Module内部）
        /// </summary>
        public string RecoverMethod { get; set; }
        private string TraceId = "TraceId";//调用链唯一标识
        private string GlobalTraceId = "GlobalTraceId";
        /*
         * 事务开始前
         */
        public void OnActionExecuting(BaseModule context)
        {

        }
        /*
         * 业务执行后
         */
        public void OnActionExecuted(BaseModule context)
        {
            if (context.ActionResult.Status)
            {
                context.Input.TryGetValue(GlobalTraceId, out string globalId);
                context.Input.TryGetValue(TraceId, out string traceId);
                Dictionary<string, string> response = new Dictionary<string, string>();
                response.Add("globalTraceId", globalId);
                response.Add("traceId", traceId);
                response.Add("recover", this.RecoverMethod);
                context.InvokeProcessor("Anno.Plugs.DTransaction", "DTransaction", "SagaSub", response);
            }
        }
        /*
         * 业务异常后(冲正)
         */
        public void OnException(Exception ex, BaseModule context)
        {
            context.Input.TryGetValue(GlobalTraceId, out string globalId);
            Dictionary<string, string> response = new Dictionary<string, string>();
            response.Add("globalTraceId", globalId);
            context.InvokeProcessor("Anno.Plugs.DTransaction", "DTransaction", "SagaRecovery", response);
        }
    }
}
