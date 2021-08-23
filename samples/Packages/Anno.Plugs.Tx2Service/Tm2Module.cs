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

namespace Anno.Plugs.Tx2Service
{
    public class Tm2Module: BaseModule
    {

        [SagaSub(RecoverMethod = "Tm2_action_Recover")]
        public string Tm2_action(string name)
        {
            return $"Hi {name} I am Tm2_action.";
        }
        /// <summary>
        ///sagaRlt
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Tm2_action_Recover(string name)
        {
            return $"Tm2_action_Recover:{name}";
        }
    }
}
