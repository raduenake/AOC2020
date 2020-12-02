using System;
using System.Text.RegularExpressions;

namespace _2
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("input.txt");

            var rx = new Regex(@"^(\d+)-(\d+)\s([a-z]):\s(.*)$");
            var correct = 0;
            var correct2 = 0;

            var line = file.ReadLine();
            while (line != null)
            {
                var match = rx.Match(line);
                if (match.Success)
                {
                    var min = int.Parse(match.Groups[1].Value);
                    var max = int.Parse(match.Groups[2].Value);
                    var ch = match.Groups[3].Value[0];
                    var pass = match.Groups[4].Value;

                    var rx1 = new Regex($"[{ch}]");
                    var chMatch = rx1.Matches(pass).Count;
                    if (chMatch >= min && chMatch <= max)
                    {
                        correct++;
                    }

                    if ((pass[min - 1] == ch && pass[max - 1] != ch) || (pass[min - 1] != ch && pass[max - 1] == ch))
                    {
                        correct2++;
                    }
                }
                line = file.ReadLine();
            }

            Console.WriteLine($"Correct policy 1: {correct}");
            Console.WriteLine($"Correct policy 2: {correct2}");
        }
    }
}
