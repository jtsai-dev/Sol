using System;
using System.Collections.Generic;
using System.Linq;

namespace LeetCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(ConsecutiveNumbersSum(5));
            //ArrayPairSum(new int[] { 1, 4, 3, 2 });
            //var matrix = new NumMatrix(new int[5, 5] {
            //    { 3,0,1,4,2},
            //    { 5,6,3,2,1},
            //    { 1,2,0,1,5},
            //    { 4,1,0,1,7},
            //    { 1,0,3,0,5},
            //});
            //Console.WriteLine(matrix.SumRegion(2, 1, 4, 3));
            //NextPermutation(new int[] { 3, 2, 1 });
            //var tree = new TreeNode(3) { left = new TreeNode(9), right = new TreeNode(20) { left = new TreeNode(15), right = new TreeNode(7) } };
            //var tree = new TreeNode(4)
            //{
            //    left = new TreeNode(2)
            //    {
            //        left = new TreeNode(1),
            //        right = new TreeNode(3)
            //    },
            //    right = new TreeNode(7)
            //    {
            //        left = new TreeNode(6),
            //        right = new TreeNode(9)
            //    }
            //};
            //InvertTree(tree);
            //Console.WriteLine(CheckRecord("PPALLP"));
            //FullJustify(new string[] { "This", "is", "an", "example", "of", "text", "justification." }, 16);
            //FullJustify(new string[] { "Science", "is", "what", "we", "understand", "well", "enough", "to", "explain", "to", "a", "computer.", "Art", "is", "everything", "else", "we", "do" }, 20);
            //Console.WriteLine(FirstMissingPositive(new int[] { 7, 8, 9, 11, 12 }));
        }

        // 829--
        public static int ConsecutiveNumbersSum(int N)
        {
            var end = N % 2 == 1 ? (N + 1) / 2 : N / 2;
            var tick = 1;
            for (var i = 1; i < end; i++)
            {
                for(var j = i + 1; j <= end; j++)
                {
                    if ((i + j) * (j - i + 1) / 2 == N)
                        tick++;
                }
            }
            return tick;
        }

        // 844
        public static bool BackspaceCompare(string S, string T)
        {
            Func<string, string> func = (str) =>
            {
                var r = "";
                for (var i = 0; i < str.Length; i++)
                {
                    if (str[i] != '#')
                        r += str[i];
                    else if (r.Length > 0)
                        r = r.Substring(0, r.Length - 1);
                }
                return r;
            };
            return func(S.TrimStart('#')) == func(T.TrimStart('#'));
        }

        // 561
        public static int ArrayPairSum(int[] nums)
        {
            var list = nums.OrderBy(p => p).ToList();
            var sum = 0;
            for (var i = 0; i < list.Count(); i += 2)
            {
                sum += list[i];
            }
            return sum;
        }

        // 137
        public static int SingleNumber(int[] nums)
        {
            return nums.GroupBy(p => p).First(p => p.Count() == 1).Key;
        }

        // 304--
        public class NumMatrix
        {
            private int[,] _matrix;
            public NumMatrix(int[,] matrix)
            {
                _matrix = matrix;
            }

            public int SumRegion(int row1, int col1, int row2, int col2)
            {
                var sum = 0;
                for (var r = row1; r <= row2; r++)
                {
                    for (var c = col1; c <= col2; c++)
                    {
                        sum += _matrix[r, c];
                    }
                }

                return sum;
            }
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

        // 11
        public int MaxArea(int[] height)
        {
            var max = 0;
            for (var i = 0; i < height.Length - 1; i++)
            {
                for (var j = i + 1; j < height.Length; j++)
                {
                    var area = (j - i) * (height[i] < height[j] ? height[i] : height[j]);
                    max = max < area ? area : max;
                }
            }
            return max;
        }

        // 226
        public static TreeNode InvertTree(TreeNode root)
        {
            if (root == null) return root;

            var row = new List<TreeNode>() { root };
            while (row.Count() > 0)
            {
                var newRow = new List<TreeNode>();
                row.ForEach(p =>
                {
                    if (p?.left != null)
                    {
                        newRow.Add(p.left);
                    }
                    if (p?.right != null)
                    {
                        newRow.Add(p.right);
                    }

                    var temp = p.left;
                    p.left = p.right;
                    p.right = temp;

                    row = newRow;
                });
            }
            return root;
        }

        // 101
        public static bool IsSymmetric(TreeNode root)
        {
            if (root == null) return true;
            var row = new List<TreeNode>() { root };
            while (row.Count() > 0 && !row.All(p => p == null))
            {
                var nums = new List<int?>();
                var reverse = new List<int?>();
                var newRow = new List<TreeNode>();
                row.ForEach(p =>
                {
                    if (p?.left != null)
                    {
                        nums.Add(p.left.val);
                        reverse.Insert(0, p.left.val);
                        newRow.Add(p.left);
                    }
                    else
                    {
                        nums.Add(null);
                        reverse.Insert(0, null);
                    }

                    if (p?.right != null)
                    {
                        nums.Add(p.right.val);
                        reverse.Insert(0, p.right.val);
                        newRow.Add(p.right);
                    }
                    else
                    {
                        nums.Add(null);
                        reverse.Insert(0, null);
                    }
                });

                row = newRow;
                if (!nums.SequenceEqual(reverse))
                    return false;
            }
            return true;
        }

        // 637
        public static IList<double> AverageOfLevels(TreeNode root)
        {
            var r = new List<double>();
            if (root == null) return r;
            var list = new List<TreeNode>() { root };
            while (list.Count() > 0)
            {
                r.Add(list.Sum(p => (double)p.val) / list.Count());
                var temp = new List<TreeNode>();
                list.ForEach(p =>
                {
                    if (p.left != null)
                        temp.Add(p.left);
                    if (p.right != null)
                        temp.Add(p.right);
                    list = temp;
                });
            }
            return r;
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
            var r = new List<IList<int>>();
            if (root == null) return r;
            r.Add(new List<int>() { root.val });

            var row = new List<TreeNode>() { root };
            while (row.Count() > 0)
            {
                var nums = new List<int>();
                var temp = new List<TreeNode>();
                row.ForEach(p =>
                {
                    if (p.left != null)
                    {
                        nums.Add(p.left.val);
                        temp.Add(p.left);
                    }
                    if (p.right != null)
                    {
                        nums.Add(p.right.val);
                        temp.Add(p.right);
                    }
                });
                if (nums.Count() > 0)
                    r.Add(nums);
                row = temp;
            }

            return r;
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
