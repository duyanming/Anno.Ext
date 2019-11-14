using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Test
{
    using Anno.LRUCache;

    public class LruCacheTest
    {
        static LRUCache<String, String> cache = new LRUCache<String, String>(10, 20);
        public static void Handle()
        {
            for (int i = 0; i < 10; i++)
            {
                cache.Set(i.ToString(), i.ToString() + ":Value");
            }
        to:
            Console.WriteLine($"Total:{cache.Count}");
            var key = Console.ReadLine();
            if (key == "all")
            {
                foreach (var item in cache._linkedList)
                {
                    //cache.TryGet(item, out string itemValue);
                    Console.WriteLine(item);
                }
                goto to;
            }
            if (key == "r")
            {
                Console.WriteLine($"Plase in put Remove key:");
                var removeKey = Console.ReadLine();
                cache.Remove(removeKey);
                goto to;
            }
            if (key == "c")
            {
                cache.Dispose();
                goto to;
            }
            if (cache.TryGet(key, out String x))
            {
                Console.WriteLine(x);
            }
            else
            {
                Console.WriteLine("Not Found!");
                cache.Set(key, key + ":Value");
            }
            goto to;
        }
    }
}
