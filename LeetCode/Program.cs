using System;
using System.Collections.Generic;
using System.Linq;

namespace LeetCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var origin = new int[,] { { 1, 2 }, { 3, 4 } };
            MatrixReshape(origin, 1, 4);
            //var l1 = new ListNode(1) { next = new ListNode(2) { next = new ListNode(6) { next = new ListNode(3) { next = new ListNode(4) { next = new ListNode(5) { next = new ListNode(6) } } } } } };
            //RemoveElements(l1, 6);

            //var l1 = new ListNode(1) { next = new ListNode(2) { next = new ListNode(4) } };
            //var l2 = new ListNode(1) { next = new ListNode(3) { next = new ListNode(4) } };
            //var t = MergeTwoLists(l1, l2);


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
        }

        public static int[,] MatrixReshape(int[,] nums, int r, int c)
        {
            var result = new int[r, c];
            var i = 0;
            var j = 0;
            foreach (var row in nums)
            {
                result[j, i] = row;
                if (i < r-1)
                {
                    i++;
                }
                else
                {
                    i = 0;
                    j++;
                }
            }
            return result;
        }

        // 
        public static int[] SortedSquares(int[] A)
        {
            var nums = new List<int>();
            foreach (var num in A)
            {
                nums.Add(num * num);
            }
            nums.Sort();
            return nums.ToArray();
        }

        // 
        public static ListNode RemoveElements(ListNode head, int val)
        {
            var result = new ListNode(0);
            var cur = result;
            var next = head;
            while (next != null)
            {
                if (next.val != val)
                {
                    cur.next = new ListNode(next.val);
                    cur = cur.next;
                }
                next = next.next;
            }

            return result;
        }

        public static ListNode MergeTwoLists(ListNode l1, ListNode l2)
        {
            var next1 = l1;
            var next2 = l2;
            ListNode result = new ListNode(0);
            var cur = result;
            while (next1 != null && next2 != null)
            {
                if (next1.val < next2.val)
                {
                    cur.next = new ListNode(next1.val);
                    next1 = next1.next;
                }
                else
                {
                    cur.next = new ListNode(next2.val);
                    next2 = next2.next;
                }
                cur = cur.next;
            }
            cur.next = next1 == null ? next2 : next1;

            return result.next;
        }


        // 28
        public static int StrStr(string haystack, string needle)
        {
            if (haystack == needle || string.IsNullOrEmpty(needle))
                return 0;
            var arr = haystack.Split(needle);
            if (arr.Length > 1)
                return arr[0].Length;
            return -1;
        }

        // 67--
        public static string AddBinary(string a, string b)
        {
            Int64 r = 0;
            r = Convert.ToInt64(a, 2) + Convert.ToInt64(b, 2);
            return Convert.ToString(r, 2);
        }

        // 25
        public class ListNode
        {
            public int val;
            public ListNode next;
            public ListNode(int x) { val = x; }
        }
        public static ListNode ReverseKGroup(ListNode head, int k)
        {
            var list = new List<int>();
            while (head != null)
            {
                list.Add(head.val);
                head = head.next;
            }
            var nums = new List<int>();
            for (var i = 0; i < Math.Ceiling((double)list.Count / k); i++)
            {
                var temp = list.Skip(i * k).Take(k);
                if (temp.Count() == k)
                {
                    temp = temp.Reverse();
                }
                nums.AddRange(temp);
            }
            nums.Reverse();
            ListNode r = null;
            foreach (var item in nums)
            {
                r = new ListNode(item) { next = r };
            }
            return r;
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
