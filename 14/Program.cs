using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


namespace _14
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = System.IO.File.ReadAllText("input.txt")
                .Split("\r\n")
                .ToImmutableList();

            var memV1 = new ConcurrentDictionary<ulong, ulong>();
            var memV2 = new ConcurrentDictionary<ulong, ulong>();

            var mask = "";
            foreach (var instr in input)
            {
                var instrSplit = instr.Split("=").Select(s => s.Trim()).ToList();
                if (instrSplit[0].StartsWith("mask"))
                {
                    mask = instrSplit[1];
                }
                if (instrSplit[0].StartsWith("mem"))
                {
                    var addr = ulong.Parse(instrSplit[0].Replace("mem[", "").Replace("]", ""));
                    var value = ulong.Parse(instrSplit[1]);

                    var maskedValue = applyMask(value, mask);
                    memV1.AddOrUpdate(addr, maskedValue, (_, _) => maskedValue);

                    Parallel.ForEach(applyMaskV2(addr, mask, 0), (newAddr) =>
                    {
                        memV2.AddOrUpdate(newAddr, value, (_, _) => value);
                    });
                }
            }

            Console.WriteLine($"{memV1.Aggregate((ulong)0, (ulong s, KeyValuePair<ulong, ulong> kv) => s += kv.Value)}");
            Console.WriteLine($"{memV2.Aggregate((ulong)0, (ulong s, KeyValuePair<ulong, ulong> kv) => s += kv.Value)}");

            ulong applyMask(ulong no, string mask)
            {
                var result = no;
                for (int i = 0; i < mask.Length; i++)
                {
                    result = mask[mask.Length - 1 - i] switch
                    {
                        '1' => result |= ((ulong)1 << i),
                        '0' => result &= ~((ulong)1 << i),
                        _ => result
                    };
                }
                return result;
            }

            IEnumerable<ulong> applyMaskV2(ulong no, string mask, int idx)
            {
                if (idx == mask.Length)
                {
                    return new List<ulong> { 0 };
                }

                var result = new List<ulong>();
                var resultInt = applyMaskV2(no, mask, idx + 1);

                foreach (var partialResultValue in resultInt)
                {
                    var r = mask[mask.Length - 1 - idx] switch
                    {
                        '0' => new[] { partialResultValue | (no & ((ulong)1 << idx)) },
                        '1' => new[] { partialResultValue | ((ulong)1 << idx) },
                        'X' => new[] { partialResultValue, partialResultValue | ((ulong)1 << idx) },
                        _ => new ulong[] { }
                    };
                    result.AddRange(r);
                }

                return result;
            }
        }
    }
}
