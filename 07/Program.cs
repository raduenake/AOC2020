using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _7
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("input.txt");

            var lines = file.ReadToEnd()
                .Split("\r\n")
                .Select(l =>
                {
                    var kv = l.Replace(" contain ", ":").Split(":");

                    return new KeyValuePair<string, IEnumerable<string>>(
                        kv[0].Replace(" bags", ""),
                        kv[1].Split(", ").Select(s => s.Replace(".", "")));
                })
                .ToList();

            var solColors = new List<string>();
            var level = lines.Where(l => l.Value.Any(lv => lv.Contains("shiny gold"))).ToList();
            do
            {
                solColors.AddRange(level.Select(l => l.Key));

                level = lines.Where(l =>
                    l.Value.Any(lv => level.Any(lv0 => lv.Contains(lv0.Key))
                )).ToList();
            }
            while (level.Count() > 0);

            Console.Write($"=== total distinct outermost: {solColors.Distinct().Count()}");
            Console.WriteLine();

            ulong result = 0;
            var currentBagContents = new List<KeyValuePair<string, string>>();
            var currentBag = lines.Where(l => l.Key.Contains("shiny gold")).ToList();
            
            do
            {
                currentBagContents = currentBag.SelectMany(lvl => lvl.Value.Select(llv =>
                {
                    var m = Regex.Match(llv, @"^(\d+)\s(.+)(\sbag|\sbags)$").Groups;
                    var p = currentBagContents.Any(sb => sb.Key == lvl.Key) ? currentBagContents.Find(sb => sb.Key == lvl.Key).Value : "1";
                    return new KeyValuePair<string, string>(
                        m[2].Value,
                        $"{p}*{m[1].Value}");
                }))
                .Where(l => !string.IsNullOrEmpty(l.Key))
                .ToList();

                var currentBagCount = currentBagContents.Aggregate((ulong)0, (s, bag) => s += bag.Value.Split("*").Aggregate((ulong)1, (p, e) => p * ulong.Parse(e)));
                result += currentBagCount;

                Console.Write($"{string.Join("===", currentBagContents.Select(cb => cb.Key + ":" + cb.Value))}");
                Console.WriteLine($"===current count:{currentBagCount}");
                
                currentBag = lines.Where(l =>
                    currentBagContents.Any(lv0 => l.Key.Contains(lv0.Key))
                ).ToList();
            }
            while (currentBag.Count() > 0);

            Console.WriteLine($"===total count:{result}");
        }
    }
}
