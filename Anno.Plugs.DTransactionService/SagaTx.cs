/****************************************************** 
Writer: Du YanMing-admin
Mail:dym880@163.com
Create Date: 8/23/2021 1:59:47 PM
Functional description： SagaTx
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;


namespace Anno.Plugs.DTransactionService
{
    public class SagaTx
    {
        public string sagaGlobalId { get; set; }
        public string sagaId { get; set; }
        public string recover { get; set; }
        public Dictionary<string, string> sagaInput { get; set; }
        public string sagaRlt { get; set; } = string.Empty;
    }
}
