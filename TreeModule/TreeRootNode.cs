﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeModule
{
    public class TreeRootNode<T>: IDisposable
    {
        public readonly Tree<T> Container;
        public readonly LinkedList<TreeNode<T>> Children = new LinkedList<TreeNode<T>>();
        
        public int Level { get; protected set; }

        public bool IsRoot => Level == 0;
        
        public T Value;

        public TreeRootNode(Tree<T> container)
        {
            Container = container;
            Level = 0;
        }

        public TreeRootNode(Tree<T> container, T value)
        {
            Container = container;
            Level = 0;
            Value = value;
        }

        /// <summary>
        /// Добавляет новый элемент по значению 
        /// </summary>
        public virtual TreeNode<T> Add(T value)
        {
            var newNode = new TreeNode<T>(this, value);
            Children.AddLast(newNode);

            return newNode;
        }

        /// <summary>
        /// Вставляет новый узел в элемент 
        /// </summary>
        public TreeNode<T> Add(TreeNode<T> newNode)
        {
            Children.AddLast(newNode);
            return newNode;
        }

        /// <summary>
        /// Добавляет новые узлы, получаемые из значений 
        /// </summary>
        public virtual void AddRange(params T[] values)
        {
            AddRange(values
                .Select(v => new TreeNode<T>(this, v))
                .ToArray());
        }
        
        /// <summary>
        /// Добавляет набор узлов 
        /// </summary>
        public void AddRange(params TreeNode<T>[] nodes)
        {
            foreach (var node in nodes)
                Children.AddLast(node);
        }

        /// <summary>
        /// Удаляет дочерние элементы (с поиском по значению)
        /// </summary>
        /// <param name="value">Значение дочернего элемента</param>
        /// <param name="deep">Необхоимо ли обходить дочерние элементы элемента</param>
        /// <param name="firstOnly">Удаляет только первый элемент</param>
        public void RemoveChild(T value, bool deep = false, bool firstOnly = true)
        {
            var treeNodes = Children.Where(ch => ch.Value.Equals(value)).ToArray();
            for (var i = 0; i < treeNodes.Length; i++)
            {
                treeNodes[i].Remove(deep);
                if(firstOnly)
                    break;
            }
        }

        /// <summary>
        /// Устанаваливает узел текущим
        /// </summary>
        public TreeRootNode<T> SetCurrent()
        {
            Container.Current = this;
            return this;
        }

        /// <summary>
        /// Вызывает переданный метод для всех дочерних объетов (deep = TRUE: опускается на уровень ниже
        /// <param name="args">[0] - deep: обход дочерних элементов</param>
        /// </summary>
        public void ChildrenInvoke(Action<TreeRootNode<T>, object[]> action, params object[] args)
        {
            var deep = (bool) args[0];
            
            if (deep)
            {
                foreach (var child in Children)
                    child.ChildrenInvoke(action, args); 
            }
            
            action.Invoke(this, args);
        }
        
        #region IDisposable Support
        public bool DisposedValue { get; protected set; }

        protected virtual void Dispose(bool disposing, bool deep = true)
        {
            if (DisposedValue) 
                return;
            
            if (disposing)
            {
                Value = default;
                
                ChildrenInvoke(Dispose, deep);
                Children.Clear();
                
                Container.Dispose();
            }
            DisposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected static void Dispose(TreeRootNode<T> treeNode, object[] args)
        {
            treeNode.Dispose(true, (bool) args[0]);
            GC.SuppressFinalize(treeNode);
        }
        #endregion
    }
}