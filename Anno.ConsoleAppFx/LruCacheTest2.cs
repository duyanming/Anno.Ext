using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.ConsoleAppFx
{
    using Anno.LRUCache;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class LruCacheTest2
    {
        static LRUCache<String, String> cache = new LRUCache<String, String>(1024, 60);
        public static void Handle()
        {
            Console.WriteLine($"Plase in put Cache Number:");
            int.TryParse(Console.ReadLine(), out int number);

            Stopwatch sw = Stopwatch.StartNew();
            Parallel.For(0, number, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 }, i =>
                  {
                      cache.Set(i.ToString(), i.ToString() + ":Value");
                  });
            sw.Stop();
            Console.WriteLine($"运行时间：{sw.ElapsedMilliseconds}/ms,TPS:{number * 1000 / sw.ElapsedMilliseconds}");

        Handle:
            Console.WriteLine($"请输入持续时间:");
            int.TryParse(Console.ReadLine(), out int secodes);
            DateTime deadline = DateTime.Now.AddSeconds(secodes);
            sw.Restart();
            long c = 0;
            while (DateTime.Now < deadline)
            {
                Parallel.For(0, 5, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, i =>
                {
                    var x = new Random().Next(number - 1024, number - 1);
                    cache.TryGet(x.ToString(), out string value);
                    cache.Set(x.ToString(), value+x);
                    Interlocked.Increment(ref c);
                });
            }
            sw.Stop();
            Console.WriteLine($"运行时间：{sw.ElapsedMilliseconds}/ms,TPS:{c * 1000 / sw.ElapsedMilliseconds}");
            goto Handle;
        }
    }
}
