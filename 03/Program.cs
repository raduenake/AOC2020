using System;
using System.Linq;

namespace _3
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("inputg.txt");
            var lines = file.ReadToEnd()
                .Split("\n")
                .Select(l => l.Replace("\r", "").Replace("\n", ""))
                .ToList();

            var trees = (a: 0, b: 0, c: 0, d: 0, e: 0);
            var idx = (a: 0, b: 0, c: 0, d: 0, e: 0);
            var step = (a: 1, b: 3, c: 5, d: 7, e: 1);
            var tree = '#';

            for (var i = 1; i < lines.Count; i++)
            {
                var line = lines[i];
                idx.a = (idx.a + step.a) % line.Length;
                idx.b = (idx.b + step.b) % line.Length;
                idx.c = (idx.c + step.c) % line.Length;
                idx.d = (idx.d + step.d) % line.Length;
                idx.e = i % 2 == 0 ? ((idx.e + step.e) % line.Length) : idx.e;
                trees.a += line[idx.a] == tree ? 1 : 0;
                trees.b += line[idx.b] == tree ? 1 : 0;
                trees.c += line[idx.c] == tree ? 1 : 0;
                trees.d += line[idx.d] == tree ? 1 : 0;
                trees.e += i % 2 == 0 ? (line[idx.e] == tree ? 1 : 0) : 0;
            }

            Console.WriteLine($"Found: {trees} .. Product: {(ulong) trees.a * (ulong) trees.b * (ulong) trees.c * (ulong) trees.d * (ulong) trees.e}");
        }
    }
}
