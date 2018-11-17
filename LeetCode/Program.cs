using System;
using System.Collections.Generic;
using System.Linq;

namespace LeetCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(RotateString("abcde", "abced"));
        }

        // 796
        public static bool RotateString(string A, string B)
        {
            if (A.Length != B.Length)
                return false;

            return (A + A).Contains(B);
        }

        // 443
        public static int Compress(char[] chars)
        {
            if (chars.Length < 2)
                return chars.Length;

            var list = new List<char>();
            var tick = 1;
            for (var i = 1; i < chars.Length; i++)
            {
                if (chars[i] == chars[i - 1])
                    tick++;
                else
                {
                    list.Add(chars[i - 1]);
                    if (tick > 1)
                        list.AddRange(tick.ToString().ToCharArray());
                    tick = 1;
                }

                if (i == chars.Length - 1)
                {
                    list.Add(chars[i]);
                    if (tick > 1)
                        list.AddRange(tick.ToString().ToCharArray());
                }
            }
            for (var i = 0; i < list.Count(); i++)
            {
                chars[i] = list[i];
            }
            var sum = list.Count();
            return sum;
        }
    }
}
