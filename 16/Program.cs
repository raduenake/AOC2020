using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _16
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt")
                .Split("\r\n\r\n");

            var r = new Regex(@"^([\w+,\s]+):\s(\d+)-(\d+)\sor\s(\d+)-(\d+)$");
            var ranges = input[0].Split("\r\n")
                .Select(l =>
                {
                    var matches = r.Match(l);
                    Func<int, bool> predicate = (input) =>
                        (input >= int.Parse(matches.Groups[2].Value) &&
                        input <= int.Parse(matches.Groups[3].Value)) ||
                        (input >= int.Parse(matches.Groups[4].Value) &&
                        input <= int.Parse(matches.Groups[5].Value));
                    return (fieldName: matches.Groups[1].Value, fieldPredicate: predicate);
                }).ToList();

            var neighborTickets = input[2].Split("\r\n").Skip(1).Select(line =>
                line.Split(",").Select(stringValue => int.Parse(stringValue)).ToImmutableList()
            ).ToImmutableList();

            var invalidTicketsValues = neighborTickets.SelectMany(ticketValues =>
                ticketValues.Where(fieldValue => !ranges.Any(rangePredicate => rangePredicate.fieldPredicate(fieldValue)))
            );
            Console.WriteLine($"{invalidTicketsValues.Sum()}");

            var validNeighborTickets = neighborTickets.Where(ticketValues =>
                ticketValues.All(fieldValue => ranges.Any(r => r.fieldPredicate(fieldValue)))
            ).ToImmutableList();

            // index is the value position in the ticket
            var ticketFieldValues = ranges.Select((_, position) =>
                (position, posValues: validNeighborTickets.Select(validTicketFields => validTicketFields[position])))
            .ToImmutableList();

            // for each field(range), valid value positions in ticket
            var fieldValidPositions = ranges.Select((rangePredicate, field) =>
                (
                    field,
                    positions:
                        ticketFieldValues.Where(tfv => tfv.posValues.All(fv => rangePredicate.fieldPredicate(fv)))
                        .Select(tfv => (tfv.position)).ToImmutableList()
                )
            )
            .OrderBy(fvc => fvc.positions.Count()) // consider the least options first
            .ToImmutableList();

            var fieldToPosMapping = Enumerable.Repeat(-1, ranges.Count()).ToArray();
            foreach (var fvp in fieldValidPositions)
            {
                fieldToPosMapping[fvp.field] = fvp.positions.First(position => !fieldToPosMapping.Contains(position));
            }

            var ticket = input[1].Split("\r\n").Skip(1).SelectMany(ticketLine =>
                ticketLine.Split(",").Select(f => int.Parse(f))
            ).ToList();

            // the first 6 fields are called "departure ..."; one can extract field idx and apply it to fieldToPosMapping
            var fieldIds = ranges.Select((r, ix) => (id: ix, range: r))
                .Where(riv => riv.range.fieldName.StartsWith("departure"))
                .Select(riv => riv.id);

            var prod = fieldIds.Select(fieldPos => ticket[fieldToPosMapping[fieldPos]])
                .Aggregate((ulong)1, (ulong product, int ticketField) => product *= (ulong)ticketField);
            Console.WriteLine($"{prod}");
        }
    }
}
