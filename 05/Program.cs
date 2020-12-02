using System;
using System.Linq;
using System.Collections;

namespace _5
{
    class Program
    {
        static void Main(string[] args)
        {
            // control
            // Start by considering the whole range, rows 0 through 127.
            // F means to take the lower half, keeping rows 0 through 63.
            // B means to take the upper half, keeping rows 32 through 63.
            // F means to take the lower half, keeping rows 32 through 47.
            // B means to take the upper half, keeping rows 40 through 47.
            // B keeps rows 44 through 47.
            // F keeps rows 44 through 45.
            // The final F keeps the lower of the two, row 44
            // Start by considering the whole range, columns 0 through 7.
            // R means to take the upper half, keeping columns 4 through 7.
            // L means to take the lower half, keeping columns 4 through 5.
            // The final R keeps the upper of the two, column 5.
            // SeatID = Multiply the row by 8, then add the column
            if (SeatId("FBFBBFFRLR") != 357)
            {
                Console.WriteLine("Fail!");
            };

            var file = System.IO.File.OpenText("input.txt");
            var bp = file.ReadToEnd()
                .Split("\r\n")
                .Select(bp => SeatId(bp))
                .OrderBy(bp => bp)
                .ToList();

            int empty = 0;
            // some of the seats at the very front and back of the plane don't exist on this aircraft, so they'll be missing from your list as well.
            // skip front & end row (as per requirements)
            // the seats with IDs +1 and -1 from yours will be in your list
            foreach (var seat in bp)
            {
                if (!bp.Any(b => b == seat + 1) && bp.Any(b => b == seat + 2))
                {
                    empty = seat + 1;
                    break;
                }
            }

            Console.WriteLine($"Max ID: {bp.Last()}; Missing ID: {empty}");

            int SeatId(string incode)
            {
                // a nice idea would be to convert to binary
                int row = 0;
                int col = 0;
                int maxrow = 127;
                int maxcol = 7;

                for (int i = 0; i < incode.Length; i++)
                {
                    var midrow = (maxrow - row) / 2 + (maxrow - row) % 2;
                    var midcol = (maxcol - col) / 2 + (maxcol - col) % 2;
                    switch (incode[i])
                    {
                        case 'F':
                            maxrow -= midrow;
                            break;
                        case 'B':
                            row += midrow;
                            break;
                        case 'L':
                            maxcol -= midcol;
                            break;
                        case 'R':
                            col += midcol;
                            break;
                    }
                }
                return row * 8 + col;
            }
        }
    }
}
