namespace NWF.Shared.Utilities
{
    public class TreeNode(string key)
    {
        public string Name { get; set; } = key;
        public TreeNodeCollection Children { get; set; } = [];
    }

    /// <remarks>
    /// TODO: Not tested; Pending more generalized path splitter handling
    /// </remarks>
    public class TreeNodeCollection: Dictionary<string, TreeNode>
    {
        private const char PathSplitter = '/';

        public static TreeNodeCollection Parse(IEnumerable<string> paths)
        {
            TreeNodeCollection rootCollection = [];
            foreach (var path in paths)
                rootCollection.AddEntry(path, 0, PathSplitter);
            return rootCollection;
        }

        #region Methods
        public void AddEntry(string path, int beginIndex, char pathSplitter)
        {
            if (beginIndex < path.Length)
            {
                string key;
                int engIndex;

                engIndex = path.IndexOf(pathSplitter, beginIndex);
                if (engIndex == -1)
                    engIndex = path.Length;
                key = path[beginIndex..engIndex];
                if (!string.IsNullOrEmpty(key))
                {
                    TreeNode nextItem;

                    if (ContainsKey(key))
                        nextItem = this[key];
                    else
                    {
                        nextItem = new TreeNode(key);
                        Add(key, nextItem);
                    }
                    // Now add the rest to the new item's children
                    nextItem.Children.AddEntry(path, engIndex + 1, pathSplitter);
                }
            }
        }
        #endregion
    }
    public static class TextFormatHelper
    {
        /// <summary>
        /// Prints a tree-like structure in console-friendly text format
        /// </summary>
        public static void PrintTree(TreeNode node, string indent, bool last)
        {
            Console.Write(indent + "+- " + node.Name);
            indent += last ? "   " : "|  ";

            int i = 0;
            int childrenCount = node.Children.Count;
            foreach ((string Key, TreeNode Node) in node.Children.OrderBy(c => c.Key))
            {
                PrintTree(Node, indent, i == childrenCount - 1);
                i++;
            }
        }
    }
}
