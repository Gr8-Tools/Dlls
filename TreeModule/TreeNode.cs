using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TreeModule.Extensions;
using TreeModule.Utils;
using UnityEngine;

namespace TreeModule
{
    public class TreeNode<T>: TreeRootNode<T>
    {
        public TreeRootNode<T> Parent { get; private set; }
        
        public TreeNode(TreeRootNode<T> parent) : base(parent.Container)
        {
            Parent = parent;
            Level = parent.Level + 1;
        }

        public TreeNode(TreeRootNode<T> parent, T value) : base(parent.Container, value)
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
        /// Возвращает следущий узел 
        /// </summary>
        public virtual TreeRootNode<T> GetNext(int step = 1, TrackingDirection preferedDirection = TrackingDirection.Null)
        {
            return GetNext(this, ref step, ref preferedDirection);
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
        
        #region STATIC

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
        /// Получает следующий/предыдущий/дочрений/Родительский узел и устанавливает его текущим
        /// </summary>
        public static TreeRootNode<T> GetNext(TreeNode<T> currentNode, ref int step, ref TrackingDirection preferedDirection)
        {
            if (step == 0)
                return currentNode;

            step = Mathf.Clamp(step, -1, 1);
            
            if (preferedDirection == TrackingDirection.Null)
                preferedDirection = currentNode.Container.DefaultTrackingDirection;

            switch (preferedDirection)
            {
                case TrackingDirection.ToBrother:
                    return GetNextBrother(currentNode, step);
                case TrackingDirection.ToChild:
                    return GetNextChild(currentNode, step);
                default: return null;
            }
        }

        /// <summary>
        /// Возвращает следующий/предыдущий узел того же уровня 
        /// </summary>
        public static TreeRootNode<T> GetNextBrother(TreeNode<T> currentNode, int step, bool deep = true)
        {
            if (currentNode.IsRoot)
                return null;
            
            var linkedNode = currentNode.Parent.Children.Find(currentNode);
            var newLinkedNode = step > 0
                ? linkedNode.Next
                : linkedNode.Previous;

            var nextNode = newLinkedNode?.Value.SetCurrent();

            if (!deep || nextNode != null)
                return nextNode;

            return GetNextChild(currentNode, step, false);
        }

        /// <summary>
        /// Возвращает дочерний или родительский узел (в зависисмости от направления)  
        /// </summary>
        public static TreeRootNode<T> GetNextChild(TreeNode<T> currentNode, int step, bool deep = true)
        {
            if (step < 0)
                return currentNode.IsRoot
                    ? null
                    : currentNode.Parent.SetCurrent();

            var nextNode = currentNode.Children.Any()
                ? currentNode.Children.First.Value.SetCurrent()
                : null;

            if (!deep || nextNode != null)
                return nextNode;

            while ((nextNode = GetNextBrother(currentNode, step, false)) == null)
            {
                if (currentNode.Parent.IsRoot)
                    return null;
                
                currentNode = (TreeNode<T>)currentNode.Parent;
            }

            return nextNode;
        }

        #endregion

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
        
        #endregion
    }
}