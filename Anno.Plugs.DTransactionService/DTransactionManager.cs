/****************************************************** 
Writer: Du YanMing-admin
Mail:dym880@163.com
Create Date: 8/23/2021 3:10:03 PM
Functional description： DTransactionManager
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.DTransactionService
{
    using System.Collections.Concurrent;
    public static class DTransactionManager
    {
        public static ConcurrentDictionary<string, SagaTxs> Sagas = new ConcurrentDictionary<string, SagaTxs>();

    }
    public class SagaTxs
    {
        public ConcurrentStack<SagaTx> Sagas { get; set; } = new ConcurrentStack<SagaTx>();
        public bool Success { get; set; } = true;
        public DateTime Deadline { get; set; }=DateTime.Now.AddSeconds(30);
    }
}