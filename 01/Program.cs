using System;
using System.Linq;
using System.Diagnostics;

namespace _1
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var year = 2020;
            var file = System.IO.File.OpenText("input.txt");
            var lines = file.ReadToEnd();

            var numbers = lines.Split('\n').Select(s => int.Parse(s)).OrderBy(n => n).ToList();
            
            var no = 0;
            var no2 = 0;
            var runs = 0;
            // no = numbers.FirstOrDefault(n =>
            // {
            //     runs++;
            //     no2 = 0;
            //     no2 = numbers.FirstOrDefault(n1 => numbers.Any(n2 => n2 == (year - n) - n1));

            //     if (no2 != 0)
            //     {
            //         return numbers.Any(n3 => n3 == year - n - no2);
            //     }
            //     else
            //     {
            //         return false;
            //     }
            // });

            for (int i = 0; i < numbers.Count(); i++)
            {
                var match = false;
                runs++;

                no = numbers[i];
                for (int j = i; j < numbers.Count(); j++)
                {
                    no2 = numbers[j];
                    for (int k = j; k < numbers.Count(); k++)
                    {
                        if (year == no + no2 + numbers[k])
                        {
                            match = true;
                            break;
                        }
                    }
                    if (match)
                    {
                        break;
                    }
                }
                if (match)
                {
                    break;
                }
            }

            sw.Stop();

            if (no != 0 && no2 != 0)
            {
                Console.WriteLine($"1: {no}, 2: {no2}, 3: {year - no - no2}, *: {no * no2 * (year - no - no2)}");
                Console.WriteLine($"Found it in {runs} runs");
            }
            else
            {
                Console.WriteLine("Tough luck!");
            }
            Console.WriteLine($"Processed in [{sw.ElapsedMilliseconds}]ms");
        }
    }
}
