using System;
using System.Linq;
using ExtensionsLibrary;
using JetBrains.Annotations;
using TreeGraph;

namespace CodeGeneration
{
    public class BaseGeneratorNode: TreeNode<string>
    {
        public BaseGeneratorNode(Tree<string> tree) : base(tree){ }
        public BaseGeneratorNode(Tree<string> tree, string value) : base(tree, value){ }

        public BaseGeneratorNode(TreeNode<string> parent) : base(parent){ }
        public BaseGeneratorNode(TreeNode<string> parent, string value) : base(parent, value){ }

        /// <summary>
        /// Return Value with tabulation count as NodeLevel 
        /// </summary>
        public string TabbedValue
        {
            get
            {
                var level = Level;
                switch (level)
                {
                    case -1:
                        throw new Exception("Can't generate tabbed value");
                    case 0:
                        return Value;
                    default:
                        var tabString = new string('\t', level);
                        var tabedRows = Value.GetRows()
                            .Select(r => $"{tabString}{r}");

                        return string.Join(Environment.NewLine, tabedRows);
                }
            }
        }

        public string GetCode()
        {
            var result = $"{TabbedValue}{Environment.NewLine}";
            if (!Children.Any())
                return result;

            var tabString = new string('\t', Level);
            result += tabString + "{" + $"{Environment.NewLine}";

            result = Children.OfType<BaseGeneratorNode>()
                .Aggregate(result,
                    (current, node) => current + node.GetCode());
            //result += $"{Environment.NewLine}{tabString}" + "}" + Environment.NewLine;
            result += $"{tabString}" + "}" + Environment.NewLine;
            return result;
        }

        public static BaseGeneratorNode Create(Tree<string> tree, [CanBeNull] string value)
        {
            return string.IsNullOrEmpty(value) 
                ? null 
                : new BaseGeneratorNode(tree, value);
        }
        
        public static BaseGeneratorNode Create(TreeNode<string> parent, [CanBeNull] string value)
        {
            return string.IsNullOrEmpty(value) 
                ? null 
                : new BaseGeneratorNode(parent, value);
        }
    }
}