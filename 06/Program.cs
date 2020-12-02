using System;
using System.Linq;

namespace _6
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("input.txt");
            var lines = file.ReadToEnd()
                .Split("\r\n\r\n")
                .ToList();

            var answers = lines.Sum(l =>
            {
                var ppl = l.Split("\r\n");
                var answers = l.Replace("\r\n", "");
                return answers.Distinct().Count(c_no => answers.Count(c => c == c_no) == ppl.Length);
            });

            Console.WriteLine($"{answers}");
        }
    }
}
