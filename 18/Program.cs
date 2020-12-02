using System;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace _18
{
    public class Op
    {
        public ulong Value { get; private set; }

        public Op(ulong input)
        {
            Value = input;
        }
        public static Op operator +(Op a, Op b)
        {
            return new Op(a.Value + b.Value);
        }

        public static Op operator -(Op a, Op b)
        {
            return new Op(a.Value * b.Value);
        }

        public static Op operator *(Op a, Op b)
        {
            return new Op(a.Value + b.Value);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = System.IO.File.ReadAllText("input.txt")
                .Split("\r\n")
                .ToImmutableList();

            var options = ScriptOptions.Default.WithReferences(typeof(Op).Assembly);

            var result = new StringBuilder("0UL");
            var result2 = new StringBuilder("0UL");

            foreach (var line in input)
            {
                var newInput = Regex.Replace(line, @"\d+", match => $"(new _18.Op({match.Value}))");
                newInput = newInput.Replace('*', '-');
                result.Append($"+({newInput}).Value");

                newInput = newInput.Replace('+', '*');
                result2.Append($"+({newInput}).Value");
            }

            var r1 = CSharpScript.EvaluateAsync(result.ToString(), options).Result;
            var r2 = CSharpScript.EvaluateAsync(result2.ToString(), options).Result;

            Console.WriteLine($"{r1}");
            Console.WriteLine($"{r2}");
        }
    }
}
