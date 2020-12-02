using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace _23
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = "389547612".Select(c => int.Parse("" + c)).ToImmutableList();

            var sw = new Stopwatch();

            // Part 1
            sw.Start();
            var cup1 = Play(new LinkedList<int>(input), 100);
            var result = "";
            foreach (var _ in Enumerable.Range(0, 8))
            {
                cup1 = cup1.NextOrFirst();
                result += cup1.Value;
            }
            sw.Stop();
            Console.WriteLine($"LL:{result}:{sw.ElapsedMilliseconds}");

            sw.Restart();
            var part1 = PlayWithArrays(input, 100);
            result = "";
            var ix = 1;
            foreach (var _ in Enumerable.Range(0, 8))
            {
                result += part1[ix];
                ix = part1[ix];
            }
            Console.WriteLine($"ARR:{result}:{sw.ElapsedMilliseconds}");
            sw.Stop();

            // Part 2
            sw.Restart();
            input = input.AddRange(Enumerable.Range(10, 1_000_001 - 10));
            cup1 = Play(new LinkedList<int>(input), 10_000_000);
            sw.Stop();
            Console.WriteLine($"LL:{1UL * (ulong)cup1.NextOrFirst().Value * (ulong)cup1.NextOrFirst().NextOrFirst().Value}:{sw.ElapsedMilliseconds}");

            sw.Restart();
            var part2 = PlayWithArrays(input, 10_000_000);
            Console.WriteLine($"ARR:{1UL * (ulong)part2[1] * (ulong)part2[part2[1]]}:{sw.ElapsedMilliseconds}");
            sw.Stop();

            LinkedListNode<int> Play(LinkedList<int> cups, int rounds)
            {
                // create an index to have a "fast" node search
                var cupsIndex = new Dictionary<int, LinkedListNode<int>>();
                var s = cups.First;
                while (s != null)
                {
                    cupsIndex.Add(s.Value, s);
                    s = s.Next;
                }

                var currentRound = 0;
                var currentCup = cups.First;
                do
                {
                    currentRound++;
                    var pickUp = new List<LinkedListNode<int>> {
                        currentCup.NextOrFirst(),
                        currentCup.NextOrFirst().NextOrFirst(),
                        currentCup.NextOrFirst().NextOrFirst().NextOrFirst()};

                    foreach (var pick in pickUp)
                    {
                        cups.Remove(pick);
                    }

                    var destinationCupValue = (int) currentCup.Value - 1;
                    while (destinationCupValue < 1 ||
                        pickUp.Any(p => p.Value == destinationCupValue) ||
                        destinationCupValue == currentCup.Value
                    )
                    {
                        destinationCupValue -= 1;
                        if (destinationCupValue < 1)
                        {
                            destinationCupValue = cupsIndex.Count();
                        }
                    }

                    currentCup = currentCup.NextOrFirst();
                    var target = cupsIndex[destinationCupValue];
                    foreach (var pick in pickUp)
                    {
                        cups.AddAfter(target, pick);
                        target = target.NextOrFirst();
                    }
                } while (currentRound < rounds);

                return cupsIndex[1];
            }

            int[] PlayWithArrays(IEnumerable<int> cups, int rounds)
            {
                // 1 based array
                int[] cupsArray = new int[cups.Count() + 1];
                var cupToNextCups = cups.Zip(cups.Skip(1).Union(new[] { cups.First() }), (cup, nextCup) => (cup, nextCup));

                foreach (var cupToNextCup in cupToNextCups) {
                    cupsArray[cupToNextCup.cup] = cupToNextCup.nextCup;
                }

                var currentRound = 0;
                var currentCup = cups.First();
                do
                {
                    currentRound++;
                    var pickUp = new[] {
                        cupsArray[currentCup],
                        cupsArray[cupsArray[currentCup]],
                        cupsArray[cupsArray[cupsArray[currentCup]]]
                    };

                    var destinationCup = currentCup - 1U;
                    while (destinationCup < 1 ||
                        pickUp.Any(p => p == destinationCup) ||
                        destinationCup == currentCup
                    )
                    {
                        destinationCup -= 1;
                        if (destinationCup < 1)
                        {
                            destinationCup = cupsArray.Length - 1;
                        }
                    }

                    cupsArray[currentCup] = cupsArray[pickUp[2]];
                    cupsArray[pickUp[2]] = cupsArray[destinationCup];
                    cupsArray[destinationCup] = pickUp[0];

                    currentCup = cupsArray[currentCup];
                } while (currentRound < rounds);

                return cupsArray;
            }
        }
    }

    static class CircularLinkedList
    {
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }
    }
}
