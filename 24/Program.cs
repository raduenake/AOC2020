using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace _24
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("input.txt");
            var input = file.ReadToEnd()
                .Split("\r\n")
                .ToList();

            var dir = new Dictionary<string, Complex>();
            dir.Add("e", new Complex(2, 0));
            dir.Add("se", new Complex(1, -1));
            dir.Add("sw", new Complex(-1, -1));
            dir.Add("w", new Complex(-2, 0));
            dir.Add("ne", new Complex(1, 1));
            dir.Add("nw", new Complex(-1, 1));

            var black = new HashSet<Complex>();
            foreach (var l in input)
            {
                var tile = new Complex(0, 0);
                var dirKey = string.Empty;
                foreach (var c in l)
                {
                    dirKey += c;
                    if (dir.ContainsKey(dirKey))
                    {
                        tile += dir[dirKey];
                        dirKey = string.Empty;
                    }
                }
                if (black.Contains(tile))
                {
                    black.Remove(tile);
                }
                else
                {
                    black.Add(tile);
                }
            }
            Console.WriteLine($"{black.Count()}");

            for (int i = 0; i < 100; i++)
            {
                var newBlack = new HashSet<Complex>();
                var expanse = black.SelectMany(blackTile => dir.Select(nbr => blackTile + nbr.Value)).ToList();
                foreach (var tile in expanse)
                {
                    var existingNbrCount = dir.Select(nbr => tile + nbr.Value)
                        .Where(nbr => black.Contains(nbr))
                        .Count();
                    if ((black.Contains(tile) && existingNbrCount >= 1 && existingNbrCount <= 2) || 
                        (!black.Contains(tile) && existingNbrCount == 2))
                    {
                        newBlack.Add(tile);
                    }
                }
                black = newBlack;
            }
            Console.WriteLine($"{black.Count()}");
        }
    }
}
