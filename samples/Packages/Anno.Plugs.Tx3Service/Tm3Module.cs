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

namespace Anno.Plugs.Tx3Service
{
    public class Tm3Module: BaseModule
    {
        [SagaSub(RecoverMethod = "Tm3_action_Recover")]
        public string Tm3_action(string name)
        {
            return $"Hi {name} I am Tm3_action.";
        }

        [SagaSub(RecoverMethod = "Tm3_action_Recover")]
        public string Tm3_action_Error(string name)
        {
            throw new Exception("Tm3_action_Error");
        }
        /// <summary>
        ///sagaRlt
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Tm3_action_Recover(string name)
        {
            return $"Tm3_action_Recover:{name}";
        }        
    }
}
