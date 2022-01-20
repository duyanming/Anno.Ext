using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.ConsoleAppFx
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
            Console.WriteLine($"请输入持续时间:");
            int.TryParse(Console.ReadLine(), out int secodes);
            DateTime deadline = DateTime.Now.AddSeconds(secodes);
            sw.Restart();

            while (DateTime.Now < deadline)
            {
                List<Task> tasks = new List<Task>();
                for (int x = 0; x < 5; x++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        int i = new Random().Next(0, number - 1);
                        cache.TryGet(i.ToString(), out string value);
                        cache.Set(i.ToString(), value + i);
                    }));
                }
                Task.WaitAll(tasks.ToArray());
            }
            sw.Stop();
            Console.WriteLine($"运行时间：{sw.ElapsedMilliseconds}/ms,TPS:{number * 1000 / sw.ElapsedMilliseconds}");
            goto Handle;
        }
    }
}
