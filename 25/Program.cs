using System;

namespace _25
{
    class Program
    {
        static void Main(string[] args)
        {
            ulong card = 16616892;
            ulong door = 14505727;
            ulong modulus = 20201227;

            ulong value = 1L;
            ulong loop = 0L;
            while (value != card)
            {
                value = (value * 7) % modulus;
                loop++;
            }

            ulong key = 1L;
            for (uint i = 0; i < loop; i++)
            {
                key = (key * door) % modulus;
            }
            
            Console.WriteLine($"{key}");
        }
    }
}
