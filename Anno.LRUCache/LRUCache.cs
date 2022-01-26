using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Anno.LRUCache
{
    /// <summary>
    /// 默认缓存长度255 五秒检查一次 30分钟未访问的 销毁
    /// 超过总长度的会被销毁，超时未访问的也销毁
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class LRUCache<TKey, TValue> : IDisposable
    {
        private bool disposed;
        private CancellationTokenSource cancelToken;
        const int DEFAULT_CAPACITY = 255;

        int _capacity;
        double _seconds = 60 * 30;//30分钟
        ReaderWriterLockSlim _locker;
        ConcurrentDictionary<TKey, LruCacheObj<TKey, TValue>> _dictionary;
        /// <summary>
        /// Key 链表
        /// </summary>
        public LinkedList<TKey> _linkedList;
        private Thread expireTask;
        public LRUCache() : this(DEFAULT_CAPACITY) { }
        ~LRUCache()
        {
            Dispose(false);
        }

        public LRUCache(int capacity)
        {
            _locker = new ReaderWriterLockSlim();
            _capacity = capacity > 0 ? capacity : DEFAULT_CAPACITY;
            _dictionary = new ConcurrentDictionary<TKey, LruCacheObj<TKey, TValue>>();
            _linkedList = new LinkedList<TKey>();
            cancelToken = new CancellationTokenSource();
            Expire();
        }
        public LRUCache(int capacity, double expireSeconds) : this(capacity)
        {
            if (expireSeconds > 0)
            {
                _seconds = expireSeconds;
            }
        }

        public void Set(TKey key, TValue value)
        {
            _locker.EnterWriteLock();
            try
            {
                _dictionary[key] = new LruCacheObj<TKey, TValue>(key, value);
                _linkedList.Remove(key);
                _linkedList.AddFirst(key);
                if (_linkedList.Count > _capacity)
                {
                    _dictionary.TryRemove(_linkedList.Last.Value, out LruCacheObj<TKey, TValue> tValue);
                    _linkedList.RemoveLast();
                }
            }
            finally
            {
                if (_locker.IsWriteLockHeld)
                {
                    _locker.ExitWriteLock();
                }
            }
        }
        /// <summary>
        /// 获取Value 自定义是否延长 有效期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="slide">true  延长有效期; false 不延长有效期</param>
        /// <returns></returns>
        public bool TryGet(TKey key, out TValue value, bool slide)
        {
            LruCacheObj<TKey, TValue> cacheObj;
            bool b = _dictionary.TryGetValue(key, out cacheObj);
            if (b)
            {
                _locker.EnterWriteLock();
                try
                {
                    if (slide)
                    {
                        cacheObj.LastVisitTime = DateTime.Now;
                    }
                    _linkedList.Remove(key);
                    _linkedList.AddFirst(key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (_locker.IsWriteLockHeld)
                    {
                        _locker.ExitWriteLock();
                    }
                }
            }
            if (cacheObj != null)
            {
                value = cacheObj.CacheObjValue;
            }
            else
            {
                value = default(TValue);
            }
            return b;

        }
        /// <summary>
        /// 获取Value 并延长 有效期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(TKey key, out TValue value)
        {
            var rlt = TryGet(key, out value, true);
            return rlt;
        }
        /// <summary>
        /// 根据指定Key 移除 缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key)
        {
            _locker.EnterWriteLock();
            try
            {
                _dictionary.TryRemove(key, out LruCacheObj<TKey, TValue> tValue);
                _linkedList.Remove(key);
            }
            finally
            {
                if (_locker.IsWriteLockHeld)
                {
                    _locker.ExitWriteLock();
                }
            }
        }
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        public int Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                if (value > 0 && _capacity != value)
                {
                    _locker.EnterWriteLock();
                    try
                    {
                        _capacity = value;
                        while (_linkedList.Count > _capacity)
                        {
                            _dictionary.TryRemove(_linkedList.Last.Value, out LruCacheObj<TKey, TValue> tValue);
                            _linkedList.RemoveLast();
                        }
                    }
                    finally
                    {
                        if (_locker.IsWriteLockHeld)
                        {
                            _locker.ExitWriteLock();
                        }
                    }
                }
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values.Select(it => it.CacheObjValue).ToList();

        private void Expire()
        {
            expireTask = new Thread(() =>
            {
            Expire:
                try
                {
                    while (cancelToken.Token.IsCancellationRequested == false)
                    {
                        CacheClear();
                        Thread.Sleep(5000);
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception)
                {
                    goto Expire;
                }
            });
            expireTask.Name = "Lru_expireTask";
            expireTask.IsBackground = true;
            expireTask.Start();
        }

        private void CacheClear()
        {
            var now = DateTime.Now;
            var objs = _dictionary.Values.Where(it => (now - it.LastVisitTime).TotalSeconds > _seconds).ToList();
            if (objs.Count > 0)
            {
                try
                {
                    if (_locker.TryEnterWriteLock(-1))
                    {
                        foreach (var obj in objs)
                        {
                            LruCacheObj<TKey, TValue> cacheObj;
                            _dictionary.TryRemove(obj.CacheObjKey, out cacheObj);
                            _linkedList.Remove(obj.CacheObjKey);
                        }
                    }

                }
                finally
                {
                    if (_locker.IsWriteLockHeld)
                    {
                        _locker.ExitWriteLock();
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    cancelToken.Cancel();
                    _locker.Dispose();
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
