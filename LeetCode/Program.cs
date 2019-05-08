using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeetCode
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine();
        }

        // 31--
        public static void NextPermutation(int[] nums)
        {
            var flag = false;
            for (var i = nums.Length - 1; i > 0; i--)
            {
                if (nums[i] > nums[i - 1])
                {
                    var temp = nums[i];
                    nums[i] = nums[i - 1];
                    nums[i - 1] = temp;
                    flag = true;
                }
            }
            if (!flag)
            {
                for (var i = 0; i < nums.Length - 1; i++)
                {
                    for (var j = i + 1; j < nums.Length; j++)
                    {
                        if (nums[i] > nums[j])
                        {
                            var temp = nums[i];
                            nums[i] = nums[j];
                            nums[j] = temp;
                        }
                    }
                }
            }
        }

        // 68--
        public static IList<string> FullJustify(string[] words, int maxWidth)
        {
            var r = new List<string>();
            var row = new List<string>();

            Func<List<string>, string> handleStr = (args) =>
            {
                var spaceCount = maxWidth - args.Sum(p => p.Length);
                var perSpace = spaceCount / (args.Count() - 1);
                var rest = spaceCount % (args.Count() - 1);
                var s = "";
                for (var i = 0; i < args.Count(); i++)
                {
                    s += (args[i] + "".PadRight(perSpace) + (rest > 0 ? " " : ""));
                    rest--;
                }
                return s.Substring(0, maxWidth);
            };
            for (var i = 0; i < words.Length; i++)
            {
                var length = row.Sum(p => p.Length) + row.Count() - 1;
                if (length + words[i].Length + 1 < maxWidth)
                    row.Add(words[i]);
                else
                {
                    r.Add(handleStr(row));
                    row = new List<string>() { words[i] };
                }
            }
            r.Add(string.Join(" ", row).PadRight(maxWidth));
            return r;
        }

        // 647--
        public static int CountSubstrings(string s)
        {
            var r = 0;
            for (var i = 0; i < s.Length; i++)
            {
                for (var j = 1; j < s.Length - i + 1; j++)
                {
                    var temp = s.Substring(i, j);
                    if (temp == string.Join("", temp.Reverse()))
                        r++;
                }
            }
            return r;
        }

        // 862--
        public static int ShortestSubarray(int[] A, int K)
        {
            var length = int.MaxValue;
            for (var i = 0; i < A.Length; i++)
            {
                var j = i;
                var sum = A[j];
                while (j < A.Length)
                {
                    if (sum >= K)
                    {
                        length = Math.Min(j - i + 1, length);
                        break;
                    }
                    j++;
                    if (j < A.Length)
                        sum += A[j];
                }
            }
            return length == int.MaxValue ? -1 : length;
        }

        #region class
        public class TreeNode
        {
            public int val;
            public TreeNode left;
            public TreeNode right;
            public TreeNode(int x) { val = x; }
        }
        public class ListNode
        {
            public int val;
            public ListNode next;
            public ListNode(int x) { val = x; }
        } 
        #endregion
    }
}
