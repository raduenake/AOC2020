using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace _17
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("input.txt");
            var input = file.ReadToEnd()
                .Split("\r\n")
                .ToList();

            var inputSlice = input.SelectMany((l, x) =>
                l.Select((c, y) => (c, y)).Where(ay => ay.c == '#')
                .Select(ay => (x: x, y: ay.y, z: 0, w: 0))
            );

            var sw = new Stopwatch();
            sw.Start();

            Solve(inputSlice, 6, false);
            Console.WriteLine($"--> {sw.ElapsedMilliseconds}");

            sw.Restart();

            Solve(inputSlice, 6, true);
            Console.WriteLine($"--> {sw.ElapsedMilliseconds}");

            sw.Stop();

            void Solve(IEnumerable<(int, int, int, int)> input, int iterations = 6, bool theFourthDimension = false)
            {
                var neighbors = Enumerable.Range(-1, 3)
                .SelectMany(x => Enumerable.Range(-1, 3)
                        .SelectMany(y => Enumerable.Range(-1, 3)
                            .SelectMany(z => Enumerable.Range(-1, 3)
                                .Select(w => (x: x, y: y, z: z, w: w))
                            )
                        )
                ).ToImmutableList();
                neighbors = neighbors.Remove((0, 0, 0, 0));

                var activeHyperCube = input.ToImmutableHashSet();
                int passes = 0;
                while (passes < iterations)
                {
                    passes++;

                    var rangeX = ExpanseRange(0, activeHyperCube);
                    var rangeY = ExpanseRange(1, activeHyperCube);
                    var rangeZ = ExpanseRange(2, activeHyperCube);
                    var rangeW = ExpanseRange(3, activeHyperCube);

                    var theExpanse = rangeX
                        .SelectMany(x => rangeY
                            .SelectMany(y => rangeZ
                                .SelectMany(z =>
                                    theFourthDimension switch
                                    {
                                        true => rangeW.Select(w => (x: x, y: y, z: z, w: w)),
                                        false => new List<(int x, int y, int z, int w)> { (x: x, y: y, z: z, w: 0) }
                                    })
                            )
                        ).ToImmutableHashSet();

                    activeHyperCube = theExpanse.AsParallel().Where(expanse =>
                    {
                        var nCount = neighbors.Count(nbr => activeHyperCube.Contains(CoordAdd(expanse, nbr)));
                        return activeHyperCube.Contains(expanse) switch
                        {
                            true => nCount >= 2 && nCount <= 3,
                            false => nCount == 3
                        };
                    }).ToImmutableHashSet();
                }

                Console.WriteLine($"{activeHyperCube.Count()}");
            }

            IEnumerable<int> ExpanseRange(int coordIx, IEnumerable<(int x, int y, int z, int w)> input)
            {
                var coordValues = input.Select(i => coordIx switch
                {
                    0 => i.x,
                    1 => i.y,
                    2 => i.z,
                    3 => i.w,
                    _ => 0
                }).Distinct();

                return Enumerable.Range(coordValues.Min() - 1, coordValues.Max() - coordValues.Min() + 3);
            }

            (int x, int y, int z, int w) CoordAdd((int x, int y, int z, int w) a, (int x, int y, int z, int w) b)
            {
                return (x: a.x + b.x, y: a.y + b.y, z: a.z + b.z, w: a.w + b.w);
            }
        }
    }
}
