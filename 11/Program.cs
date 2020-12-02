using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Immutable;

namespace _11
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("input.txt");
            var input = file.ReadToEnd()
                .Split("\r\n")
                .ToImmutableList();

            Stabilize(input, 4, false);
            Stabilize(input, 5, true);

            bool isOccupied(int i, int j, int diri, int dirj, ImmutableList<string> input, bool longSight = true)
            {
                var result = false;
                try
                {
                    var inext = i;
                    var jnext = j;
                    do
                    {
                        inext += diri;
                        jnext += dirj;

                        if (input[inext][jnext] == '.')
                        {
                            continue;
                        }

                        result = input[inext][jnext] == '#';
                        break;
                    } while (longSight);
                }
                catch { }
                return result;
            }

            void Stabilize(ImmutableList<string> lines, int weight, bool longSight)
            {
                var sw = new Stopwatch();
                sw.Start();

                var neighbors = new[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };

                var change = true;
                int passes = 0;

                do
                {
                    passes++;
                    change = false;

                    var newLines = lines.AsParallel().Select((l, i) =>
                        new string(l.Select((c, j) =>
                        {
                            var occ = neighbors
                                .Select(n => isOccupied(i, j, n.Item1, n.Item2, lines, longSight))
                                .Count(ocn => ocn == true);
                            return lines[i][j] switch
                            {
                                'L' => occ == 0 ? '#' : lines[i][j],
                                '#' => occ >= weight ? 'L' : lines[i][j],
                                _ => lines[i][j]
                            };
                        }).ToArray())
                    ).ToImmutableList();

                    change = lines.AsParallel().Any(l => l != newLines[lines.IndexOf(l)]);
                    lines = newLines;
                } while (change);

                sw.Stop();

                Console.WriteLine($"t: {sw.ElapsedMilliseconds}|it: {passes - 1}|a: {lines.Aggregate(0, (s, l) => s += l.Count(c => c == '#'))}");
            }
        }
    }
}
