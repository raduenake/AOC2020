using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _20
{
    public class Tile
    {
        public const string MONSTER =
@"                  # 
#    ##    ##    ###
 #  #  #  #  #  #   ";

        public ulong ID { get; set; }

        public IEnumerable<string> Contents { get; set; }

        public Tile(ulong id, IEnumerable<string> contents)
        {
            ID = id;
            Contents = new List<string>(contents);
            Border = CalculateBorder();
        }

        public (string left, string top, string right, string bottom) Border { get; private set; }

        public Tile Flip()
        {
            return new Tile(ID, Contents.Select(l => String.Concat(l.Reverse())));
        }

        public Tile Rotate()
        {
            return new Tile(ID,
                Contents.Select((_, ix) => string.Concat(Contents.Reverse().Select(l => l[ix])))
            );
        }

        private (string left, string top, string right, string bottom) CalculateBorder()
        {
            var top = Contents.First();
            var bottom = Contents.Last();
            var left = string.Concat(Contents.Select(l => l.First()));
            var right = string.Concat(Contents.Select(l => l.Last()));

            return (left, top, right, bottom);
        }

        public uint GetMonsterCount()
        {
            var monster = MONSTER.Split("\r\n");
            var count = 0U;

            for (int i = 0; i < Contents.Count() - monster.Count(); i++)
            {
                for (int j = 0; j < Contents.First().Length - monster.First().Length; j++)
                {
                    bool hasMonster = true;
                    for (int mi = 0; mi < monster.Count() && hasMonster; mi++)
                    {
                        for (int mj = 0; mj < monster.First().Length && hasMonster; mj++)
                        {
                            if (monster.Skip(mi).First()[mj] == '#' && Contents.Skip(i + mi).First()[j + mj] != '#')
                            {
                                hasMonster = false;
                            }
                        }
                    }
                    count += hasMonster ? 1 : 0;
                }
            }

            return count;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");

            var tiles = input.Split("\r\n\r\n").Select(t =>
            {
                var tc = t.Trim().Split("\r\n");
                var id = ulong.Parse(Regex.Match(tc.First(), @"^.*\s(\d+):$").Groups[1].Value);

                return new Tile(id, tc.Skip(1));
            }).ToList();

            var domain = tiles.SelectMany(t =>
            {
                var result = new List<Tile> { t, t.Flip() };
                var rot = t;
                result.AddRange(Enumerable.Range(0, 3).SelectMany(_ =>
                {
                    rot = rot.Rotate();
                    return new List<Tile> { rot, rot.Flip() };
                }));
                return result;
            }).ToList();

            var domainOptions = domain.Select((d, ix) =>
            {
                var edgeMatches = Enumerable.Range(0, 4).Select(r =>
                     domain.Select((d2, ix2) => (d2, ix2))
                         .Where(d2ix => d2ix.d2.ID != d.ID &&
                             r switch
                             {
                                 0 => d2ix.d2.Border.bottom == d.Border.top,
                                 1 => d2ix.d2.Border.top == d.Border.bottom,
                                 2 => d2ix.d2.Border.right == d.Border.left,
                                 _ => d2ix.d2.Border.left == d.Border.right
                             }
                         )
                         .Select(d2ix => d2ix.ix2)
                ).ToList();
                return new KeyValuePair<int, (IEnumerable<int> leftMatches, IEnumerable<int> topMatches, IEnumerable<int> rightMatches, IEnumerable<int> bottomMatches)>
                (
                    ix,
                    (leftMatches: edgeMatches[2], topMatches: edgeMatches[0], rightMatches: edgeMatches[3], bottomMatches: edgeMatches[1])
                );
            }).ToDictionary(x => x.Key, x => x.Value);

            var visited = new List<ulong>();
            var imgSize = (int)Math.Sqrt(tiles.Count());
            int[,] finalImage = new int[imgSize, imgSize];

            var corners = domainOptions.Where(oo => !oo.Value.leftMatches.Any() && !oo.Value.topMatches.Any());
            var usedTileIds = new List<ulong>();
            
            for (int i = 0; i < imgSize; i++)
            {
                for (int j = 0; j < imgSize; j++)
                {
                    int currentDomainIx = -1;
                    if (i == 0 && j == 0)
                    {
                        // start with a corner
                        currentDomainIx = corners.First().Key;
                    }
                    else if (i == 0)
                    {
                        // first row has no "top border"
                        var leftIx = finalImage[i, j - 1];
                        currentDomainIx = domainOptions[leftIx].rightMatches.First(rm =>
                            !usedTileIds.Contains(domain[rm].ID) &&
                            !domainOptions[rm].topMatches.Any()
                        );
                    }
                    else
                    {
                        // best option that "links" to the tile above and the left
                        var aboveIx = finalImage[i - 1, j];
                        var leftIx = j > 0 ? finalImage[i, j - 1] : -1;
                        currentDomainIx = domainOptions[aboveIx].bottomMatches.First(bm =>
                            !usedTileIds.Contains(domain[bm].ID) &&
                            (j > 0 || !domainOptions[bm].leftMatches.Any()) &&
                            (j < imgSize - 1 || !domainOptions[bm].rightMatches.Any()) &&
                            (leftIx == -1 || domainOptions[bm].leftMatches.Any(lm => lm == leftIx))
                        );
                    }

                    finalImage[i, j] = currentDomainIx;
                    usedTileIds.Add(domain[currentDomainIx].ID);
                }
            }

            var image = new List<string>();
            for (int i = 0; i < imgSize; i++)
            {
                // discard border (-2 lines)
                var partialImage = Enumerable.Range(0, domain[0].Contents.Count() - 2).Select(_ => "");
                for (int j = 0; j < imgSize; j++)
                {
                    partialImage = partialImage
                        .Select((l, ix) =>
                        {
                            // discard border (line: ix + 1 from original, and discard first and last chars from each line)
                            var cl = domain[finalImage[i, j]].Contents.Skip(ix + 1).First();
                            return l + cl.Substring(1, cl.Length - 2);
                        }).ToList();
                }
                image.AddRange(partialImage);
            }

            var bigTile = new Tile(0, image);
            
            // create possible combinations
            var imageVariants = new List<Tile> { bigTile, bigTile.Flip() };
            var rot = bigTile;
            imageVariants.AddRange(Enumerable.Range(0, 3).SelectMany(_ =>
            {
                rot = rot.Rotate();
                return new List<Tile> { rot, rot.Flip() };
            }));

            uint waterCount = 0;
            foreach (var imageVariant in imageVariants)
            {
                var monsterCount = (uint)imageVariant.GetMonsterCount();
                // at least one monster was found
                if (monsterCount > 0)
                {
                    waterCount = imageVariant.Contents.Aggregate((uint)0, (s, l) => s += (uint)l.Count(c => c == '#')) -
                        monsterCount * (uint)Tile.MONSTER.Count(c => c == '#');
                    break;
                }
            }

            var cornerTiles = corners.Select(l => domain[l.Key].ID).Distinct();
            Console.WriteLine($"{cornerTiles.Aggregate(1UL, (p, e) => p *= e)}");
            Console.WriteLine($"{waterCount}");
        }
    }
}
