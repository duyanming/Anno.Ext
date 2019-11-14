using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading;

namespace MemoryCachingTest
{
    class Program
    {
        static IMemoryCache cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        int num = 0;
        static void Main(string[] args)
        {


            Console.WriteLine("Hello World!");
        }
        int GetNumber()
        {
            return cache.GetOrCreate("cache", entity =>
            {
                entity.SetAbsoluteExpiration(TimeSpan.FromSeconds(2));
                return Interlocked.Increment(ref num);
            });
        }
    }
}
