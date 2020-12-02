using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using MathNet.Numerics;

namespace _13
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadLines("input.txt")
                .Select(l => l.Replace("\r", ""))
                .ToImmutableList();

            var estimate = ulong.Parse(input[0]);
            var bus = input[1].Split(",")
                .Select((l, ix) => new KeyValuePair<int, string>(ix, l))
                .Where(t => t.Value != "x")
                .Select(kv => new KeyValuePair<int, ulong>(kv.Key, ulong.Parse(kv.Value)))
                .ToImmutableList();

            ulong time = estimate;
            ulong busNo = 0;
            do
            {
                try
                {
                    // Ugly control for multiple
                    busNo = bus.First(b => time % b.Value == 0).Value;
                    break;
                }
                catch { }
                time++;
            } while (true);
            Console.WriteLine($"{time}|{time - estimate}|{(time - estimate) * busNo}");

            // https://en.wikipedia.org/wiki/Chinese_remainder_theorem#Search_by_sieving
            ulong result = 0;
            int b0 = 0;
            int bn = 1;
            ulong skip = bus[0].Value;
            while (bn < bus.Count())
            {
                result += skip;

                var rng = bus.Where((b, ix) => ix >= b0 && ix <= bn);
                if (rng.All(b => (result + (ulong)b.Key) % b.Value == 0))
                {
                    if (rng.Count() == bus.Count())
                    {
                        break;
                    }

                    skip = (ulong)MathNet.Numerics.Euclid.LeastCommonMultiple(
                        rng.Select(b => (long)b.Value).ToList()
                    );

                    bn++;
                }
            }

            Console.WriteLine($"{result}");
        }
    }
}
