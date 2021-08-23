﻿using System;

namespace Anno.EngineData
{
    using Anno.EngineData.Filters;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// 保证一致性，入口
    /// </summary>
    public class SagaStart : Attribute, IActionFilter, IExceptionFilter, IFilterMetadata
    {
        /// <summary>
        /// 失败后的补偿方法（必须在当前Module内部）
        /// </summary>
        public string RecoverMethod { get; set; }

        private string GlobalTraceId = "GlobalTraceId";
        /*
         * 事务开始前
         */
        public void OnActionExecuting(BaseModule context)
        {
            context.Input.TryGetValue(GlobalTraceId, out string globalId);
            Dictionary<string, string> response = new Dictionary<string, string>();
            response.Add("globalTraceId", globalId);
            response.Add("recover",this.RecoverMethod);
            response.Add("sagaInput", JsonConvert.SerializeObject(context.Input));
            context.InvokeProcessor("Anno.Plugs.DTransaction", "DTransaction", "SagaStarted", response);
        }
        /*
         * 业务执行后
         */
        public void OnActionExecuted(BaseModule context)
        {
            SagaEnd(context, context.ActionResult.Status);
        }
        /*
         * 业务异常后
         */
        public void OnException(Exception ex, BaseModule context)
        {
            SagaEnd(context,false);
        }
        /// <summary>
        /// 结束通知
        /// </summary>
        /// <param name="context"></param>
        /// <param name="success"></param>
        private void SagaEnd(BaseModule context,bool success)
        {
            context.Input.TryGetValue(GlobalTraceId, out string globalId);
            Dictionary<string, string> response = new Dictionary<string, string>();
            response.Add("globalTraceId", globalId);
            string action = "SagaEnd";
            if (success == false) {
                action = "SagaRecovery";
            }
            context.InvokeProcessor("Anno.Plugs.DTransaction", "DTransaction", action, response);
        }
    }
}
