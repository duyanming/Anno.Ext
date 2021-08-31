/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/9/7 11:00:05 
Functional description： ExamModule
******************************************************/
using Anno.Const.Attribute;
using Anno.EngineData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Plugs.Tx1Service
{
    public class Tm1Module : BaseModule
    {
        [SagaStart(RecoverMethod = "Buy_Recover")]
        public string Buy(string name)
        {
            Dictionary<string, string> inputS2 = new Dictionary<string, string>();
            inputS2.Add("name", name);
            var s2Rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<string>>(this.InvokeProcessor("Anno.Plugs.Tx2", "Tm2", "Tm2_action", inputS2)).OutputData;

            Dictionary<string, string> inputS3 = new Dictionary<string, string>();
            inputS3.Add("name", name);
            var s3Rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<string>>(this.InvokeProcessor("Anno.Plugs.Tx3", "Tm3", "Tm3_action", inputS3)).OutputData;

            return $"Hi {name} I am Anno.";
        }

        [SagaStart(RecoverMethod = "Buy_Recover")]
        public string BuyError(string name)
        {
            Dictionary<string, string> inputS2 = new Dictionary<string, string>();
            inputS2.Add("name", name);
            var s2Rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<string>>(this.InvokeProcessor("Anno.Plugs.Tx2", "Tm2", "Tm2_action", inputS2)).OutputData;

            Dictionary<string, string> inputS3 = new Dictionary<string, string>();
            inputS3.Add("name", name);
            var s3Rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<string>>(this.InvokeProcessor("Anno.Plugs.Tx3", "Tm3", "Tm3_action", inputS3)).OutputData;
            throw new Exception("BuyError");
            // return $"Hi {name} I am Anno.";
        }
        [SagaStart(RecoverMethod = "Buy_Recover")]
        public string Tm3_action_Error(string name)
        {
            Dictionary<string, string> inputS2 = new Dictionary<string, string>();
            inputS2.Add("name", name);
            var s2Rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<string>>(this.InvokeProcessor("Anno.Plugs.Tx2", "Tm2", "Tm2_action", inputS2)).OutputData;

            Dictionary<string, string> inputS3 = new Dictionary<string, string>();
            inputS3.Add("name", name);
            var s3Rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<string>>(this.InvokeProcessor("Anno.Plugs.Tx3", "Tm3", "Tm3_action_Error", inputS3)).OutputData;

            throw new Exception("BuyError");
            // return $"Hi {name} I am Anno.";
        }


        [SagaStart(RecoverMethod = "Buy_Recover", ExecutionSeconds = 5)]
        public string BuyTimeOut(string name)
        {
            Dictionary<string, string> inputS2 = new Dictionary<string, string>();
            inputS2.Add("name", name);
            var s2Rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<string>>(this.InvokeProcessor("Anno.Plugs.Tx2", "Tm2", "Tm2_action", inputS2)).OutputData;

            Dictionary<string, string> inputS3 = new Dictionary<string, string>();
            inputS3.Add("name", name);
            var s3Rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<string>>(this.InvokeProcessor("Anno.Plugs.Tx3", "Tm3", "Tm3_action", inputS3)).OutputData;
            Task.Delay(10 * 1000).Wait();
            return $"Hi {name} I am Anno.";
        }

        /// <summary>
        ///sagaRlt
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Buy_Recover(string name)
        {
            return $"Buy_Recover:{name}";
        }
    }
}
