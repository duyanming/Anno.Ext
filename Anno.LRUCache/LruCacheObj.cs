using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.LRUCache
{
    internal class LruCacheObj<TKey, TValue>
    {
        internal LruCacheObj(TKey key,TValue value)
        {
            CacheObjValue = value;
            CacheObjKey = key;
        }
        internal TKey CacheObjKey { get; private set; }
        /// <summary>
        /// 缓存对象
        /// </summary>
        internal TValue CacheObjValue { get; private set; }
        /// <summary>
        /// Key 最后访问时间
        /// </summary>
        internal DateTime LastVisitTime { get; set; } = DateTime.Now;
    }
}
