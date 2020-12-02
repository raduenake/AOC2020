using System;
using System.Linq;
using System.Collections.Generic;

namespace _9
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("input.txt");

            var lines = file.ReadToEnd()
                .Split("\r\n")
                .Select(l => ulong.Parse(l))
                .ToList();

            ulong outoforder = 0;
            for (int i = 0; i < lines.Count(); i++)
            {
                var start = i - 25;
                if (start >= 0 && IsWeakness(lines[i], lines, start, 25))
                {
                    outoforder = lines[i];
                    break;
                }
            }

            Console.WriteLine($"Missmatch:{outoforder}");

            var result = new List<ulong>();
            int preamble = 2;
            bool setFound = false;
            
            Console.Write($"Sets of");
            while (preamble < lines.Count() && !setFound)
            {
                Console.Write($":{preamble}");
                
                for (int i = preamble; i < lines.Count(); i++)
                {
                    var start = i - preamble;
                    if (start >= 0)
                    {
                        result = lines
                            .Where((e, ix) => ix >= start && ix < start + preamble)
                            .OrderBy(e => e)
                            .ToList();

                        if (outoforder == result.Aggregate((ulong)0, (s, e) => s += e))
                        {
                            setFound = true;
                            break;
                        };
                    }
                }

                preamble++;
            }
            Console.WriteLine();

            if (setFound)
            {
                Console.WriteLine($"Weakness:{result.Min() + result.Max()}");
            }

            bool IsWeakness(ulong no, List<ulong> set, int prevStart, int prevLen)
            {
                bool isWeak = false;

                var prevNo = lines
                    .Where((e, ix) => ix >= prevStart && ix < prevStart + prevLen)
                    .OrderBy(e => e)
                    .ToList();

                if (!prevNo.Any(n1 => prevNo.Any(n2 => n2 == no - n1)))
                {
                    isWeak = true;
                };

                return isWeak;
            }
        }
    }
}