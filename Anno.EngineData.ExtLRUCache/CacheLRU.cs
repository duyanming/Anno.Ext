using System;


namespace Anno.EngineData
{
    using Anno.EngineData.Cache;
    using Anno.LRUCache;
    public class CacheLRU : CacheMiddlewareAttribute
    {
        public CacheLRU()
        {
            cacheLRU = new LRUCache<string, ActionResult>(Capacity, ExpireSeconds);
        }
        public CacheLRU(int capacity)
        {
            this.Capacity = capacity;
            cacheLRU = new LRUCache<string, ActionResult>(Capacity, ExpireSeconds);
        }
        public CacheLRU(double expireSeconds):this()
        {
            this.ExpireSeconds = expireSeconds;
            cacheLRU = new LRUCache<string, ActionResult>(Capacity, ExpireSeconds);
        }
        public CacheLRU(int capacity, double expireSeconds)
        {
            this.Capacity = capacity;
            this.ExpireSeconds = expireSeconds;
            cacheLRU = new LRUCache<string, ActionResult>(Capacity, ExpireSeconds);
        }
        public CacheLRU(int capacity, double expireSeconds,bool slidingExpiration)
        {
            this.Capacity = capacity;
            this.ExpireSeconds = expireSeconds;
            this.SlidingExpiration = slidingExpiration;
            cacheLRU = new LRUCache<string, ActionResult>(Capacity, ExpireSeconds);
        }
        private int Capacity { get; set; } = 255;
        private double ExpireSeconds { get; set; } = 10 * 60;
        /// <summary>
        /// 是否滑动过期
        /// </summary>
        private bool SlidingExpiration { get; set; } = true;
        LRUCache<string, ActionResult> cacheLRU;
        public override void RemoveCache(string key)
        {
            cacheLRU.Remove(key);
        }

        public override void SetCache(string key, ActionResult actionResult)
        {
            cacheLRU.Set(key, actionResult);
        }

        public override bool TryGetCache(string key, out ActionResult actionResult)
        {
            return cacheLRU.TryGet(key, out actionResult,SlidingExpiration);
        }
    }
}
