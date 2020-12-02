using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace _22
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = File.ReadAllText("input.txt");

            var playerDecks = file.Split("\r\n\r\n").Select(pd =>
            {
                var pdin = pd.Split("\n");
                var cards = pdin.Skip(1).Select(card => int.Parse(card));
                return cards;
            });

            // Part1
            var p1 = new Queue<int>(playerDecks.First());
            var p2 = new Queue<int>(playerDecks.Last());
            var winner = P1WinsGame(p1, p2) ? p1 : p2;
            var score = winner.Select((e, ix) => (q: (winner.Count() - ix), e)).Aggregate(0, (s, e) => s += e.q * e.e);
            Console.WriteLine($"{score}");

            // Part 2
            p1 = new Queue<int>(playerDecks.First());
            p2 = new Queue<int>(playerDecks.Last());
            winner = P1WinsGame(p1, p2, true) ? p1 : p2;
            score = winner.Select((e, ix) => (q: (winner.Count() - ix), e)).Aggregate(0, (s, e) => s += e.q * e.e);
            Console.WriteLine($"{score}");

            bool P1WinsGame(Queue<int> p1Deck, Queue<int> p2Deck, bool isRecursive = false)
            {
                HashSet<string> p1DeckKeyHistory = new HashSet<string>();
                HashSet<string> p2DeckKeyHistory = new HashSet<string>();
                while (p1Deck.Any() && p2Deck.Any())
                {
                    var p1DeckKey = string.Join("_", p1Deck.Select(c => c));
                    var p2DeckKey = string.Join("_", p2Deck.Select(c => c));
                    if (isRecursive && (p1DeckKeyHistory.Contains(p1DeckKey) || p2DeckKeyHistory.Contains(p2DeckKey)))
                    {
                        return true;
                    }
                    else
                    {
                        p1DeckKeyHistory.Add(p1DeckKey);
                        p2DeckKeyHistory.Add(p2DeckKey);
                    }

                    bool p1WinsRound;
                    var p1Card = p1Deck.Dequeue();
                    var p2Card = p2Deck.Dequeue();

                    p1WinsRound = (isRecursive && p1Card <= p1Deck.Count() && p2Card <= p2Deck.Count()) switch
                    {
                        true => P1WinsGame(new Queue<int>(p1Deck.Take(p1Card)), new Queue<int>(p2Deck.Take(p2Card)), isRecursive),
                        false => p1Card > p2Card
                    };

                    if (p1WinsRound)
                    {
                        p1Deck.Enqueue(p1Card);
                        p1Deck.Enqueue(p2Card);
                    }
                    else
                    {
                        p2Deck.Enqueue(p2Card);
                        p2Deck.Enqueue(p1Card);
                    }
                }
                return p1Deck.Any() ? true : false;
            }
        }
    }
}
