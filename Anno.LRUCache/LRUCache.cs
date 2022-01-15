using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        IDictionary<TKey, TValue> _dictionary;
        /// <summary>
        /// Key 最后访问时间
        /// </summary>
        IDictionary<TKey, DateTime> _keyLastVisitTimeDictionary;
        /// <summary>
        /// Key 链表
        /// </summary>
        public LinkedList<TKey> _linkedList;
        public LRUCache() : this(DEFAULT_CAPACITY) { }
        ~LRUCache()
        {
            Dispose(false);
        }

        public LRUCache(int capacity)
        {
            _locker = new ReaderWriterLockSlim();
            _capacity = capacity > 0 ? capacity : DEFAULT_CAPACITY;
            _dictionary = new Dictionary<TKey, TValue>();
            _keyLastVisitTimeDictionary = new Dictionary<TKey, DateTime>();
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
                _dictionary[key] = value;
                _keyLastVisitTimeDictionary[key] = DateTime.Now;
                _linkedList.Remove(key);
                _linkedList.AddFirst(key);
                if (_linkedList.Count > _capacity)
                {
                    _dictionary.Remove(_linkedList.Last.Value);
                    _keyLastVisitTimeDictionary.Remove(_linkedList.Last.Value);
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
            _locker.EnterUpgradeableReadLock();
            try
            {
                bool b = _dictionary.TryGetValue(key, out value);
                if (b)
                {
                    _locker.EnterWriteLock();
                    try
                    {
                        if (slide)
                        {
                            _keyLastVisitTimeDictionary[key] = DateTime.Now;
                        }
                        _linkedList.Remove(key);
                        _linkedList.AddFirst(key);
                    }
                    finally
                    {
                        if (_locker.IsWriteLockHeld)
                        {
                            _locker.ExitWriteLock();
                        }
                    }
                }
                return b;
            }
            catch { throw; }
            finally { _locker.ExitUpgradeableReadLock(); }
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
                _dictionary.Remove(key);
                _keyLastVisitTimeDictionary.Remove(key);
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
            _locker.EnterReadLock();
            try
            {
                return _dictionary.ContainsKey(key);
            }
            finally
            {
                if (_locker.IsReadLockHeld)
                {
                    _locker.ExitReadLock();
                }
            }
        }

        public int Count
        {
            get
            {
                _locker.EnterReadLock();
                try
                {
                    return _dictionary.Count;
                }
                finally
                {
                    if (_locker.IsReadLockHeld)
                    {
                        _locker.ExitReadLock();
                    }
                }
            }
        }

        public int Capacity
        {
            get
            {
                _locker.EnterReadLock();
                try
                {
                    return _capacity;
                }
                finally { _locker.ExitReadLock(); }
            }
            set
            {
                _locker.EnterUpgradeableReadLock();
                try
                {
                    if (value > 0 && _capacity != value)
                    {
                        _locker.EnterWriteLock();
                        try
                        {
                            _capacity = value;
                            while (_linkedList.Count > _capacity)
                            {
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
                finally { _locker.ExitUpgradeableReadLock(); }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                _locker.EnterReadLock();
                try
                {
                    return _dictionary.Keys;
                }
                finally
                {
                    if (_locker.IsReadLockHeld)
                    {
                        _locker.ExitReadLock();
                    }
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                _locker.EnterReadLock();
                try
                {
                    return _dictionary.Values;
                }
                finally
                {
                    if (_locker.IsReadLockHeld)
                    {
                        _locker.ExitReadLock();
                    }
                }
            }
        }

        private void Expire()
        {
            Task.Factory.StartNew(() =>
            {
            Expire:
                try
                {
                    while (cancelToken.Token.IsCancellationRequested == false)
                    {
                        _locker.EnterReadLock();
                        var keys = _keyLastVisitTimeDictionary.Keys.ToList();
                        _locker.ExitReadLock();
                        var now = DateTime.Now;
                        foreach (var key in keys)
                        {
                            DateTime dateTime;
                            _keyLastVisitTimeDictionary.TryGetValue(key, out dateTime);
                            if (dateTime != null && (now - dateTime).TotalSeconds > _seconds)
                            {
                                Remove(key);

                            }
                        }
                        Thread.Sleep(5000);
                    }
                }
                catch
                {
                    if (_locker.IsReadLockHeld)
                    {
                        _locker.ExitReadLock();
                    }
                    goto Expire;
                }
            }, cancelToken.Token);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    cancelToken.Cancel();
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
