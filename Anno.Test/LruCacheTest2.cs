using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Test
{
    using Anno.LRUCache;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class LruCacheTest2
    {
        static LRUCache<String, String> cache = new LRUCache<String, String>(int.MaxValue, 60);
        public static void Handle()
        {
            Console.WriteLine($"Plase in put Cache Number:");
            int.TryParse(Console.ReadLine(), out int number);

            Stopwatch sw = Stopwatch.StartNew();
            Parallel.For(0, number, i =>
            {
                cache.Set(i.ToString(), i.ToString() + ":Value");
            });
            sw.Stop();
            Console.WriteLine($"运行时间：{sw.ElapsedMilliseconds}/ms,TPS:{number * 1000 / sw.ElapsedMilliseconds}");

        Handle:
            Console.WriteLine($"Plase in put get Cache Number:");
            int.TryParse(Console.ReadLine(), out int getNumber);
            if (getNumber <= 0)
            {
                getNumber = 10;
            }
            sw.Restart();
            Parallel.For(0, getNumber, i =>
            {
                cache.TryGet(i.ToString(), out string value);
            });
            sw.Stop();
            Console.WriteLine($"运行时间：{sw.ElapsedMilliseconds}/ms,TPS:{number * 1000 / sw.ElapsedMilliseconds}");
            goto Handle;
        }
    }
}
