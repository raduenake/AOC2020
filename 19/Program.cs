using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace _19
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split("\r\n\r\n");
            var inputRulesList = input.First().Split("\r\n").Select(rl =>
            {
                var rls = rl.Split(":").Select(r => r.Trim());
                var value = string.Empty;

                var opt = Enumerable.Empty<string>();
                if (rls.Last().StartsWith('"'))
                {
                    value = rls.Last().Replace("\"", "");
                }
                else
                {
                    opt = rls.Last().Split("|").Select(r => r.Trim());
                }
                return (id: rls.First(), value, opt);
            }).ToList();

            var text = input.Last().Split("\r\n");

            // Part1 - build rule - works for input rules loops
            var rule0 = GetRuleValues("0");
            Console.WriteLine($"{text.Count(t => rule0.Contains(t))}");

            // Part2 - rules have loops
            inputRulesList = inputRulesList.Select(r => r.id switch
            {
                "8" => ("8", "", new List<string> { "42", "42 8" }),
                "11" => ("11", "", new List<string> { "42 31", "42 11 31" }),
                _ => r
            }).ToList();
            Console.WriteLine($"{text.Count(t => MatchRule("0", t))}");

            List<string> GetRuleValues(string ruleId)
            {
                var ruleValues = new List<string>();
                var rule = inputRulesList.First(r => r.id == ruleId);

                if (rule.value != string.Empty)
                {
                    ruleValues.Add(rule.value);
                }
                else
                {
                    foreach (var ruleOption in rule.opt)
                    {
                        var ruleOptionRuleIds = ruleOption.Split(" ");
                        var listCombinations = ruleOptionRuleIds
                            .Select(ruleOptionRuleId => GetRuleValues(ruleOptionRuleId))
                            .Aggregate((v1, v2) => v1.SelectMany(v11 => v2.Select(v22 => v11 + v22)).ToList());
                        ruleValues.AddRange(listCombinations);
                    }
                }

                return ruleValues;
            }

            bool MatchRule(string ruleId, string input)
            {
                var ruleMatchStack = new Stack<(int ix, List<string> rules)>();
                ruleMatchStack.Push(new(0, new List<string>() { ruleId }));

                while (ruleMatchStack.Any())
                {
                    var checkNow = ruleMatchStack.Pop();

                    if (!checkNow.rules.Any() || checkNow.ix >= input.Length)
                    {
                        continue;
                    }

                    var rule = inputRulesList.First(r => r.id == checkNow.rules.First());
                    if (rule.value != string.Empty && input[checkNow.ix] == rule.value[0])
                    {
                        if (checkNow.rules.Count() == 1 && checkNow.ix == input.Length - 1)
                        {
                            return true;
                        }
                        
                        if (checkNow.rules.Count() > 1)
                        {
                            ruleMatchStack.Push(new(checkNow.ix + 1, checkNow.rules.Skip(1).ToList()));
                        }
                    }
                    else
                    {
                        foreach (var ruleOption in rule.opt)
                        {
                            var ruleOptionRuleIds = ruleOption.Split(" ");
                            ruleMatchStack.Push(new(checkNow.ix, ruleOptionRuleIds.Concat(checkNow.rules.Skip(1)).ToList()));
                        }
                    }
                }

                return false;
            }
        }
    }
}
