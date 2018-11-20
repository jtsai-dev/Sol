using System;
using System.Collections.Generic;
using System.Linq;

namespace LeetCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new TreeNode(3) { left = new TreeNode(9), right = new TreeNode(20) { left = new TreeNode(15), right = new TreeNode(7) } };
            //MaxDepth(tree);
            //Console.WriteLine(CheckRecord("PPALLP"));
            //FullJustify(new string[] { "This", "is", "an", "example", "of", "text", "justification." }, 16);
            //FullJustify(new string[] { "Science", "is", "what", "we", "understand", "well", "enough", "to", "explain", "to", "a", "computer.", "Art", "is", "everything", "else", "we", "do" }, 20);
            Console.WriteLine(FirstMissingPositive(new int[] { 7, 8, 9, 11, 12 }));
        }

        // 104
        public static int MaxDepth(TreeNode root)
        {
            int i = 0;
            if (root == null)
                return i;
            var nexts = new List<TreeNode>() { root };
            while (nexts != null)
            {
                i++;
                var temp = new List<TreeNode>();
                nexts.ForEach(p =>
                {
                    if (p.left != null)
                        temp.Add(p.left);
                    if (p.right != null)
                        temp.Add(p.right);
                });
                if (temp.Count() > 0)
                    nexts = temp;
                else
                    nexts = null;
            }
            return i;
        }

        public class TreeNode
        {
            public int val;
            public TreeNode left;
            public TreeNode right;
            public TreeNode(int x) { val = x; }
        }

        // 102
        public static IList<IList<int>> LevelOrder(TreeNode root)
        {
            var nums = new List<IList<int>>();
            if (root == null) return nums;
            nums.Add(new List<int>() { root.val });
            IList<int> row = new List<int>();
            GetNextLevel(new List<TreeNode>() { root }, nums);
            return nums;
        }
        public static void GetNextLevel(List<TreeNode> trees, List<IList<int>> nums)
        {
            var nodes = new List<TreeNode>();
            var temp = new List<int>();
            trees.ForEach(p =>
            {
                if (p.left != null)
                {
                    nodes.Add(p.left);
                    temp.Add(p.left.val);
                }
                if (p.right != null)
                {
                    nodes.Add(p.right);
                    temp.Add(p.right.val);
                }
            });
            if (temp.Count() > 0)
                nums.Add(temp);
            if (nodes.Count() > 0)
                GetNextLevel(nodes, nums);
        }

        // 551
        public static bool CheckRecord(string s)
        {
            return s.Count(p => p == 'A') < 2 && s.IndexOf("LL") < -1;
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

        public static int FirstMissingPositive(int[] nums)
        {
            var max = nums.Max();
            for (var i = 1; i < max; i++)
            {
                if (!nums.Contains(i))
                    return i;
            }
            return max + 1;
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
