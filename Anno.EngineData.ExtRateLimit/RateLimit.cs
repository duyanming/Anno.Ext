using System;
using Anno.RateLimit;

namespace Anno.EngineData.Limit
{
    using Anno.EngineData.Cache;
    public class RateLimit : CacheMiddlewareAttribute
    {
        private int TimeInterval { get; set; } = 1;
        private LimitingType LimitType { get; set; } = LimitingType.TokenBucket;
        private int MaxQps { get; set; } = 2000;
        private int LimitSize;
        ILimitingService limit;
        public RateLimit()
        {
            limit = LimitingFactory.Build(TimeSpan.FromSeconds(TimeInterval), LimitType, MaxQps, LimitSize);
        }
        public RateLimit(LimitingType limitType, int timeIntervalSeconds, int maxQps,int limitSize)
        {
            this.TimeInterval = timeIntervalSeconds;
            this.LimitType = limitType;
            this.MaxQps = maxQps;
            this.LimitSize = limitSize;
            limit = LimitingFactory.Build(TimeSpan.FromSeconds(TimeInterval), LimitType, MaxQps, LimitSize);
        }
        public RateLimit(LimitingType limitType) : this()
        {
            this.LimitType = limitType;
        }
        public override void RemoveCache(string key)
        {
            
        }

        public override void SetCache(string key, ActionResult actionResult)
        {
            
        }

        public override bool TryGetCache(string key, out ActionResult actionResult)
        {
            /*
             * Request()==true 则没有被限流
             */
            if (limit.Request())
            {
                actionResult = new ActionResult();
                return false;
            }
            actionResult = new ActionResult(false,null,null, "Trigger current limiting.");
            return true;
        }
    }
}
