using System;
using System.Linq;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Numerics;

namespace _12
{
    class Program
    {
        static void Main(string[] args)
        {
            var rx = new Regex(@"^(.{1})(\d+)$");
            var input = File.ReadLines("input.txt")
                .Select(l => l.Replace("\r", ""))
                .Select(l =>
                {

                    var m = rx.Match(l);
                    return new KeyValuePair<string, int>(m.Groups[1].Value, int.Parse(m.Groups[2].Value));
                })
                .ToImmutableList();

            var dir = new Dictionary<string, Complex>();
            dir.Add("N", new Complex(0, 1));
            dir.Add("S", new Complex(0, -1));
            dir.Add("E", new Complex(1, 0));
            dir.Add("W", new Complex(-1, 0));

            var rot = new Dictionary<string, Complex>();
            rot.Add("L", new Complex(0, 1));
            rot.Add("R", new Complex(0, -1));

            var loc = new Complex(0, 0);
            var d = new Complex(1, 0);
            foreach (var instr in input)
            {
                if (dir.ContainsKey(instr.Key))
                {
                    loc += dir[instr.Key] * instr.Value;
                }
                else if (rot.ContainsKey(instr.Key))
                {
                    d *= Complex.Pow(rot[instr.Key], instr.Value / 90);
                }
                else
                {
                    loc += d * instr.Value;
                }
            }
            Console.WriteLine($"{loc}:{Math.Round(Math.Abs(loc.Real) + Math.Abs(loc.Imaginary))}");

            var wp = new Complex(10, 1);
            loc = new Complex(0, 0);
            foreach (var instr in input)
            {
                if (dir.ContainsKey(instr.Key))
                {
                    wp += dir[instr.Key] * instr.Value;
                }
                else if (rot.ContainsKey(instr.Key))
                {
                    wp *= Complex.Pow(rot[instr.Key], instr.Value / 90);
                }
                else
                {
                    loc += wp * instr.Value;
                }
            }
            Console.WriteLine($"{loc}:{Math.Round(Math.Abs(loc.Real) + Math.Abs(loc.Imaginary))}");
        }
    }
}