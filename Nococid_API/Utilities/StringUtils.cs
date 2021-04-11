using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Utilities
{
    public class StringUtils
    {
        private static readonly string[] array = new string[] { "q", "Q", "w", "W", "e", "0", "E", "r", "R", "t", "T",
            "1", "y", "Y", "u", "U", "i", "2", "I", "o", "O", "p", "P", "3", "a", "A", "s", "S", "d", "4", "D",
            "f", "F", "g", "G", "5", "h", "H", "j", "J", "k", "6", "K", "l", "L", "z", "Z", "7", "x", "X", "c",
            "C", "v", "8", "V", "b", "B", "n", "N", "9", "m", "M" };
        private static readonly Random random = new Random();

        public static string GenerateRandomString(int generatedLength)
        {
            string s = "";
            for (int i = 0; i < generatedLength; i++)
            {
                s += array[random.Next(0, 61)];
            }
            return s;
        }
    }
}
