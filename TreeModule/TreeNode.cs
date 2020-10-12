using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TreeModule.Utils;
using UnityEngine;

namespace TreeModule
{
    public class TreeNode<T>: TreeRootNode<T>
    {
        public TreeRootNode<T> Parent { get; private set; }
        
        public TreeNode(Tree<T> container, TreeRootNode<T> parent) : base(container)
        {
            Parent = parent;
            Level = parent.Level + 1;
        }

        public TreeNode(Tree<T> container, TreeRootNode<T> parent, T value) : base(container, value)
        {
            Parent = parent;
            Level = parent.Level + 1;
        }

        /// <summary>
        /// Удаляет элемент из дерева
        /// </summary>
        public void Remove(bool deep = false)
        {
            if (deep)
                ChildrenInvoke(Dispose, true);
            else
                DropNote(this);
        }
        
        /// <summary>
        /// Связывает текущий узел с новым родителем
        /// </summary>
        private void ResetParent(TreeRootNode<T> newParent, [CanBeNull] LinkedListNode<TreeNode<T>> addAfterPoint)
        {
            Parent = newParent;
            
            if (addAfterPoint != null)
                Parent.Children.AddAfter(addAfterPoint, this);
            else
                Parent.Children.AddLast(this);
        }
        
        #region IDisposable Support

        protected override void Dispose(bool disposing, bool deep = true)
        {
            if (DisposedValue) 
                return;
            
            if (disposing)
            {
                Value = default;
                
                Parent.Children.Remove(this);
                Parent = null;
                
                if (deep)
                {
                    ChildrenInvoke(Dispose, true);
                }
                Children.Clear();
            }
            DisposedValue = true;
        }

        /// <summary>
        /// Устанавливает нового родителя для каждого дочернего элемента
        /// </summary>
        private static void DropNote(TreeNode<T> currentNode)
        {
            var newParent = currentNode.Parent;
            var afterPoint = newParent.Children.Find(currentNode);
            foreach (var child in currentNode.Children.ToArray())
            {
                child.ResetParent(newParent, afterPoint);
                afterPoint = newParent.Children.Find(child);
            }
            
            currentNode.Dispose(true, false);
        }

        /// <summary>
        /// Получает следующий узел и устанавливает его текущим
        /// </summary>
        public static TreeRootNode<T> GetNext(TreeNode<T> currentNode, int step = 1, TrackingDirection preferedDirection = TrackingDirection.Null)
        {
            if (step == 0)
                return currentNode;

            step = Mathf.Clamp(step, -1, 1);
            
            if (preferedDirection == TrackingDirection.Null)
                preferedDirection = currentNode.Container.DefaultTrackingDirection;

            switch (preferedDirection)
            {
                case TrackingDirection.ToBrother:
                {
                    var linkedNode = currentNode.Parent.Children.Find(currentNode);
                    var newLinkedNode = step > 0
                        ? linkedNode.Next
                        : linkedNode.Previous;

                    return newLinkedNode == null
                        ? newLinkedNode.Value.SetCurrent()
                        : null;
                }
                case TrackingDirection.ToChild:
                {
                    if (step < 0)
                        return currentNode.Parent.SetCurrent();

                    return currentNode.Children.Any()
                        ? currentNode.Children.First.Value.SetCurrent()
                        : null;
                }
                default: return null;
            }
        }
        
        #endregion
    }
}