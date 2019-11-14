using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Test
{
    using Anno.LRUCache;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class LruCacheTest1
    {
        static LRUCache<String, String> cache = new LRUCache<String, String>(int.MaxValue, 10 * 60);
        public static void Handle()
        {
        Handle:
            Console.WriteLine($"Plase in put Cache Number:");
            int.TryParse(Console.ReadLine(), out int number);

            Stopwatch sw = Stopwatch.StartNew();
            Parallel.For(0, number, i =>
            {
                cache.Set(i.ToString(), i.ToString() + ":Value");
            });
            sw.Stop();
            Console.WriteLine($"运行时间：{sw.ElapsedMilliseconds}/ms,TPS:{number * 1000 / sw.ElapsedMilliseconds}");
            goto Handle;
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
