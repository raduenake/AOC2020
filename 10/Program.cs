using System;
using System.Linq;
using System.Collections.Generic;

namespace _10
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("input.txt");
            var lines = file.ReadToEnd()
                .Split("\r\n")
                .Select(l => int.Parse(l))
                .ToList();

            lines.AddRange(new[] { 0, lines.Max() + 3 });
            var adapters = lines.OrderBy(a => a).ToList();

            // In order to use all adapters, you need to connect them to the next
            // We count all the "valid" connections (difference of 1 or 3)
            var t = (one: 0, three: 0);
            for (int i = 0, j = 1; j < adapters.Count(); i++, j++)
            {
                if (adapters[j] - adapters[i] == 1)
                {
                    t.one++;
                }
                if (adapters[j] - adapters[i] == 3)
                {
                    t.three++;
                }
            }
            Console.WriteLine($"{t}|{t.one * t.three}");

            // Count all possible "paths" from an adapter to the powerline
            // Keep track of the paths for each adapter (in the dictionary)
            Dictionary<int, ulong> paths = new Dictionary<int, ulong>();
            paths.Add(0, 1); // the powerline has one path to connect
            for (int i = 1; i < adapters.Count(); i++)
            {
                // all possible connections to previous adapters
                for (int j = i - 1; j >= 0; j--)
                {
                    // stop if current adapter can't be connected to the previous one
                    if (adapters[i] - adapters[j] > 3)
                    {
                        break;
                    }
                    paths.TryAdd(i, 0);
                    paths[i] += paths[j];
                }
            }

            Console.WriteLine($"{paths[adapters.Count() - 1]}");
        }
    }
}
