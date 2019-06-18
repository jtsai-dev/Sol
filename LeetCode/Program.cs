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
            var t = DeleteDuplicates(new ListNode(1) { next = new ListNode(1) { next = new ListNode(2) } });
            var tt = SumOfLeftLeaves(new TreeNode(3)
            {
                left = new TreeNode(9),
                right = new TreeNode(20)
                {
                    left = new TreeNode(15),
                    right = new TreeNode(7),
                },
            });
            var ttt = SumOfLeftLeaves(new TreeNode(1)
            {
                left = new TreeNode(2),
                right = new TreeNode(3)
                {
                    left = new TreeNode(15),
                    right = new TreeNode(7),
                },
            });
            Console.WriteLine();
        }


        // 83
        public static ListNode DeleteDuplicates(ListNode head)
        {
            var current = head;
            var nums = new List<int>();
            while (current != null)
            {
                if (nums.Contains(current.val))
                {
                    if (current.next != null)
                    {
                        current.val = current.next.val;
                    }
                    else
                    {
                        current = null;
                    }
                    current = current.next;
                }
                else
                {
                    nums.Add(current.val);
                    current = current.next;
                }
            }
            return head;
        }

        // 404 --
        public static int SumOfLeftLeaves(TreeNode root)
        {
            if (root == null) return 0;

            return root.left == null ? 0 : root.left.val
                + SumOfLeftLeaves(root.left) + SumOfLeftLeaves(root.right);
        }

        // 463 --
        public static int IslandPerimeter(int[][] grid)
        {
            var result = 0;
            var x = grid[0].Length;
            var y = grid.Length;
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    if (grid[i][j] == 1)
                    {
                        if (i == 0)
                        {
                            result++;
                            if (j == 0 && grid[i][j + 1] == 0)
                                result++;
                            if (grid[i + 1][y] == 0)
                                result++;
                            if (j == y && grid[i][j - 1] == 0)
                                result++;
                        }
                        if (i == x)
                        {
                            result++;
                            if (j == 0 && grid[i][j + 1] == 0)
                                result++;
                            if (grid[i - 1][y] == 0)
                                result++;
                            if (j == y && grid[i][j - 1] == 0)
                                result++;
                        }
                    }
                }
            }

            return result;
        }

        public static char FindTheDifference(string s, string t)
        {
            var sSum = s.Select(p => (int)p).Sum();
            var tSum = t.Select(p => (int)p).Sum();
            return (char)(tSum - sSum);
        }

        // 671--
        public static int FindSecondMinimumValue(TreeNode root)
        {
            var first = root.val;
            var second = 0;
            if (root.left != null)
            {
                second = Math.Min(root.left.val, root.right.val);
                if (second == first)
                    second = Math.Max(root.left.val, root.right.val);
            }
            else
                return first;
            if (first == second)
                return -1;
            return second;
        }

        // 98--
        public static bool IsValidBST(TreeNode root)
        {
            if (root == null) return true;
            if (root.left != null && root.val <= root.left.val) return false;
            if (root.right != null && root.val >= root.right.val) return false;
            return IsValidBST(root.left)
                && IsValidBST(root.right);
        }

        // 538--
        public static TreeNode ConvertBST(TreeNode root)
        {
            if (root != null)
            {
                var left = ConvertBST(root.left)?.val ?? 0;
                var right = ConvertBST(root.right)?.val ?? 0;
                root.val = root.val + left > root.val ? left : 0 + right > root.val ? right : 0;
                ConvertBST(root.left);
                ConvertBST(root.right);
            }
            return root;
        }


        // 933--
        public class RecentCounter
        {
            private static List<int> data = new List<int>();
            public RecentCounter()
            {

            }

            public int Ping(int t)
            {
                data.Add(t);
                if (data.Count < 2)
                    return data[0];
                return data.Count(p => t - data[data.Count - 2] >= 3000);
            }
        }

        // 876
        public static ListNode MiddleNode(ListNode head)
        {
            var slow = head;
            while (head != null && head.next != null)
            {
                slow = slow.next;
                head = head.next.next;
            }
            return slow;
        }

        // 107
        public static IList<IList<int>> LevelOrderBottom(TreeNode root)
        {
            if (root == null) return new List<IList<int>>();
            var result = new List<IList<int>>();
            var parent = new List<TreeNode>() { root.left, root.right };
            while (parent != null && parent.Count > 0)
            {
                parent = parent.Where(p => p != null).ToList();
                if (parent.Count > 0)
                {
                    result.Insert(0, parent.Select(p => p.val).ToList());
                    var temp = new List<TreeNode>();
                    foreach (var item in parent)
                    {
                        if (item.left != null)
                            temp.Add(item.left);
                        if (item.right != null)
                            temp.Add(item.right);
                    }
                    parent = temp;
                }
            }
            result.Add(new List<int>() { root.val });

            return result;
        }

        // 669
        public static TreeNode TrimBST(TreeNode root, int L, int R)
        {
            if (root == null) return null;

            if (root.val < L)
                return TrimBST(root.right, L, R);
            if (root.val > R)
                return TrimBST(root.left, L, R);

            root.left = TrimBST(root.left, L, R);
            root.right = TrimBST(root.right, L, R);

            return root;
        }

        // 429
        public static IList<IList<int>> LevelOrder(Node root)
        {
            if (root == null) return new List<IList<int>>();
            var result = new List<IList<int>>() { new List<int>() { root.val } };
            var children = root.children;
            while (children != null && children.Count > 0)
            {
                result.Add(children.Select(p => p.val).ToList());
                var temp = children.Select(p => p.children);
                children = new List<Node>();
                foreach (var item in temp)
                {
                    if (item != null)
                        children = children.Concat(item).ToList();
                }
            }

            return result;
        }

        // 821
        public static int[] ShortestToChar(string S, char C)
        {
            var result = new int[S.Length];
            var indexs = new List<int>();
            for (var i = 0; i < S.Length; i++)
            {
                if (S[i] == C)
                    indexs.Add(i);
                result[i] = i;
            }
            for (var i = 0; i < S.Length; i++)
            {
                result[i] = indexs.Select(p => Math.Abs(p - i)).Min();
            }

            return result.ToArray();
        }

        public class Node
        {
            public int val;
            public IList<Node> children;

            public Node() { }
            public Node(int _val, IList<Node> _children)
            {
                val = _val;
                children = _children;
            }
        }
        // 559
        public static int MaxDepth(Node root)
        {
            if (root == null)
                return 0;
            int result = 0;
            for (var i = 0; i < root.children.Count; i++)
            {
                result = Math.Max(result, MaxDepth(root.children[i]));
            }
            return result;
        }

        public static string ConvertToTitle(int n)
        {
            var result = "";
            while (n > 0)
            {
                result = (char)(65 + --n % 26) + result;
                n = (int)Math.Floor((double)n / 26);
            }

            return result;
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
