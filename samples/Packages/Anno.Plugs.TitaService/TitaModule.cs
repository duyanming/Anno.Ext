using Anno.EngineData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TitaService;

namespace Anno.Plugs.TitaService
{
    public class TitaModule : BaseModule
    {
        private readonly ITitaService titaService;
        public TitaModule(ITitaService titaService)
        {
            this.titaService = titaService;
        }
        /// <summary>
        /// 异步获取一个通知消息
        /// </summary>
        /// <param name="inputMsg"></param>
        /// <returns></returns>
        public async Task<NoticeMsg> GetNoticeMsgAsync(string inputMsg)
        {
            if (inputMsg == "1")
            {
                return await Task.FromResult(new NoticeMsg() { Message = titaService.Id });
            }
            return null;
        }
    }
    public class NoticeMsg
    {
        public string Message { get; set; }
    }
}
