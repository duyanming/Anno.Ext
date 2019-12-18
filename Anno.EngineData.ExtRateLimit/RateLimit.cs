using System;
using Anno.RateLimit;

namespace Anno.EngineData
{
    using Anno.EngineData.Cache;
    public class RateLimit : CacheMiddlewareAttribute
    {
        public TimeSpan LimitTimeSpan { get; set; } = TimeSpan.FromSeconds(1);
        public LimitingType LimitType { get; set; } = LimitingType.TokenBucket;
        public int MaxQps { get; set; } = 2000;
        private int limitSize;
        public int LimitSize
        {
            get
            {
                if (limitSize <= 0)
                {
                    limitSize = MaxQps;
                }
                return limitSize;
            }
            set
            {
                if (value > 0)
                {
                    limitSize = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(LimitSize));
                }
            }
        }
        ILimitingService limit;
        public RateLimit()
        {
            limit = LimitingFactory.Build(LimitTimeSpan, LimitType, MaxQps, limitSize);
        }

        public override void RemoveCache(int key)
        {
            
        }

        public override void SetCache(int key, ActionResult actionResult)
        {
            
        }

        public override bool TryGetCache(int key, out ActionResult actionResult)
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
