using System;
using System.Linq;
using System.Collections.Generic;

namespace _8
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("inputg.txt");
            var lines = file.ReadToEnd()
                .Split("\r\n")
                .Select(l =>
                {
                    var kv = l.Split(" ");
                    return new KeyValuePair<string, string>(kv[0], kv[1]);
                })
                .ToList();

            Console.WriteLine("----P1----");
            Run(
                lines,
                (currentInstrPtr, execInstrPtrs, program) => execInstrPtrs.Any(ins => ins == currentInstrPtr),
                (currentInstrPtr, execInstrPtrs, program) => execInstrPtrs.Any(ins => ins == currentInstrPtr));

            Console.WriteLine("----P2----");
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Key == "nop" || lines[i].Key == "jmp")
                {
                    var newlines = new List<KeyValuePair<string, string>>(lines);
                    newlines[i] = new KeyValuePair<string, string>(lines[i].Key == "nop" ? "jmp" : "nop", lines[i].Value);

                    var isResult = Run(
                        newlines,
                        (currentInstrPtr, execInstrPtrs, program) =>
                            execInstrPtrs.Any(ins => ins == currentInstrPtr) || currentInstrPtr == program.Count(),
                        (currentInstrPtr, execInstrPtrs, program) =>
                            currentInstrPtr == program.Count()
                    );
                    
                    if (isResult) 
                    {
                        Console.WriteLine($"Replaced: {i}:{lines[i]} --> {newlines[i]}");
                        break;
                    }
                }
            }

            bool Run(
                List<KeyValuePair<string, string>> program,
                Func<int, List<int>, List<KeyValuePair<string, string>>, bool> stopCondition,
                Func<int, List<int>, List<KeyValuePair<string, string>>, bool> isSolution)
            {
                var ptr = 0;
                var acc = 0;

                var execInstrPtrs = new List<int>();
                Func<int, KeyValuePair<string, string>> get = (ptr) => program[ptr];

                while (!stopCondition(ptr, execInstrPtrs, program))
                {
                    execInstrPtrs.Add(ptr);
                    var op = get(ptr);

                    var reg = int.Parse(op.Value);
                    ptr++;

                    switch (op.Key)
                    {
                        case "nop":
                            break;
                        case "acc":
                            acc += reg;
                            break;
                        case "jmp":
                            ptr += reg - 1;
                            break;
                    }
                };

                if (isSolution(ptr, execInstrPtrs, program))
                {
                    foreach (var instr in execInstrPtrs)
                    {
                        Console.WriteLine($"{execInstrPtrs.IndexOf(instr)}:{instr}: {program[instr]}");
                    }
                    Console.WriteLine($"acc:{acc}");

                    return true;
                }

                return false;
            }
        }
    }
}
