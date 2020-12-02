using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace _4
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = System.IO.File.OpenText("input.txt");
            var optFields = new List<PassportFields>() { PassportFields.cid };

            var valid = 0;
            var passes = file.ReadToEnd().Split("\r\n\r\n");
            foreach (var line in passes)
            {
                var fields =
                    line.Replace("\r\n", " ").Split(" ").Select(kv =>
                    {
                        var kvs = kv.Split(":");
                        return new KeyValuePair<PassportFields, string>(Enum.Parse<PassportFields>(kvs[0]), kvs[1]);
                    });

                valid += IsValid(fields, optFields) ? 1 : 0;
            }

            Console.WriteLine($"Valid pass: {valid}");

            bool IsValid(IEnumerable<KeyValuePair<PassportFields, string>> pass, IEnumerable<PassportFields> opt)
            {
                return Enum.GetValues<PassportFields>().All(field =>
                {
                    return opt.Contains(field) || pass.Any(f => f.Key == field && FieldIsValid(f.Key, f.Value));
                });
            }

            bool FieldIsValid(PassportFields key, string value)
            {
                switch (key)
                {
                    case PassportFields.byr:
                        return Regex.IsMatch(value, @"^\d{4}$") && int.Parse(value) >= 1920 && int.Parse(value) <= 2002;
                    case PassportFields.iyr:
                        return Regex.IsMatch(value, @"^\d{4}$") && int.Parse(value) >= 2010 && int.Parse(value) <= 2020;
                    case PassportFields.eyr:
                        return Regex.IsMatch(value, @"^\d{4}$") && int.Parse(value) >= 2020 && int.Parse(value) <= 2030;
                    case PassportFields.hgt:
                        var match = Regex.Match(value, @"^(\d+)(cm|in)$");
                        if (match.Success)
                        {
                            var no = int.Parse(match.Groups[1].Value);
                            var unit = match.Groups[2].Value;
                            return unit == "cm" ?
                                no >= 150 && no <= 193 :
                                no >= 59 && no <= 76;
                        }
                        break;
                    case PassportFields.hcl:
                        return Regex.IsMatch(value, @"^#[0-9a-f]{6}$");
                    case PassportFields.ecl:
                        return Regex.IsMatch(value, @"^(amb|blu|brn|gry|grn|hzl|oth)$");
                    case PassportFields.pid:
                        return Regex.IsMatch(value, @"^[0-9]{9}$");
                    case PassportFields.cid:
                        return true;
                }
                return false;
            }
        }

        public enum PassportFields
        {
            byr,// (Birth Year)
            iyr,// (Issue Year)
            eyr,// (Expiration Year)
            hgt,// (Height)
            hcl,// (Hair Color)
            ecl,// (Eye Color)
            pid,// (Passport ID)
            cid,// (Country ID)
        }
    }
}
