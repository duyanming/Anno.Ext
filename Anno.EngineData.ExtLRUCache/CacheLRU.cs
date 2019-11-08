using System;


namespace Anno.EngineData
{
    using Anno.EngineData.Cache;
    using Anno.LRUCache;
    public class CacheLRU : CacheMiddlewareAttribute
    {
        public CacheLRU()
        {
            cacheLRU = new LRUCache<int, ActionResult>(Capacity, ExpireSeconds);
        }
        public bool IsGetAndResetTime { get; set; } = true;
        public int Capacity { get; set; } = 255;
        public double ExpireSeconds { get; set; } = 10 * 60;
        LRUCache<int, ActionResult> cacheLRU;
        public override void RemoveCache(int key)
        {
            cacheLRU.Remove(key);
        }

        public override void SetCache(int key, ActionResult actionResult)
        {
            cacheLRU.Set(key, actionResult);
        }

        public override bool TryGetCache(int key, out ActionResult actionResult)
        {
            return cacheLRU.TryGet(key, out actionResult);
        }
    }
}
