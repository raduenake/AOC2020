using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace _15
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = System.IO.File.ReadAllText("input.txt")
                .Split(",")
                .Select(s => uint.Parse(s))
                .ToImmutableList();

            var solDict = new Dictionary<uint, uint[]>();
            uint rounds = 0;
            uint last = 0;

            var sw = new Stopwatch();
            sw.Start();

            foreach (var inNo in input)
            {
                last = inNo;
                solDict[last] = solDict.ContainsKey(last) switch
                {
                    true => new uint[2] { rounds, solDict[last][0] },
                    _ => new uint[2] { rounds, rounds }
                };

                rounds++;
            }

            while (true)
            {
                last = solDict[last][0] - solDict[last][1];
                solDict[last] = solDict.ContainsKey(last) switch
                {
                    true => new uint[2] { rounds, solDict[last][0] },
                    _ => new uint[2] { rounds, rounds }
                };

                rounds++;

                if (rounds == 2020)
                {
                    Console.WriteLine($"t:[{sw.ElapsedMilliseconds}]|r:[{rounds}]|k:[{solDict.Keys.Count()}]|a:[{last}]");
                }
                else if (rounds == 30000000)
                {
                    Console.WriteLine($"t:[{sw.ElapsedMilliseconds}]|r:[{rounds}]|k:[{solDict.Keys.Count()}]|a:[{last}]");
                    break;
                }
            }

            sw.Stop();
        }
    }
}
