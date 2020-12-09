using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeGraph
{
    /// <summary>
    /// Узел дерева
    /// </summary>
    public class TreeNode<T>: IDisposable
    {
        /// <summary>
        /// Дерево, которому принадлежит этот узел.
        /// </summary>
        public Tree<T>? Tree;
        
        /// <summary>
        /// Родительский узел дерева.
        /// Если узел корневой, то Parent == null, а Tree.RootNodes содержит текущий узел.
        /// </summary>
        public TreeNode<T>? Parent;

        /// <summary>
        /// Лист дочерних узлов
        /// </summary>
        public readonly LinkedList<TreeNode<T>> Children = new LinkedList<TreeNode<T>>();
        
        /// <summary>
        /// Значение узла
        /// </summary>
        public T Value;

        /// <summary>
        /// Уровень узла (уровень коренового узла = 0)
        /// </summary>
        public int Level
        {
            get
            {
                if (Tree == null)
                    return -1;
                if (Parent == null && Tree.RootNodes.Any(el => el.Equals(this)))
                    return 0;

                var level = 0;
                var parent = Parent;
                while (parent != null)
                {
                    parent = parent.Parent;
                    level++;
                }

                return level;
            }
        }
        
        public string? Tag
        {
            get => _tag;
            set
            {
                if(_tag == value)
                    return;
                
                if(Tree == null)
                    throw new NullReferenceException("Tree is not set!");
                Tree.RemoveTag(_tag, this);
                _tag = value;
                if(!string.IsNullOrWhiteSpace(_tag))
                    Tree.AddTag(_tag, this);
            }
        }

        private string? _tag;
        
        public TreeNode(Tree<T> tree)
        {
            MakeRoot(tree, false);
        }

        public TreeNode(Tree<T> tree, T value)
        {
            MakeRoot(tree, false);
            Value = value;
        }

        public TreeNode(TreeNode<T> parent)
        {
            AppendTo(parent, false);
        }

        public TreeNode(TreeNode<T> parent, T value)
        {
            AppendTo(parent, false);
            Value = value;
        }
        
        /// <summary>
        /// Делает текущй элемент корневым.
        /// Сбрасывает родителя.
        /// Добавляет элемент в список Tree.RootNodes
        /// </summary>
        public void MakeRoot(Tree<T> tree, bool remove = true)
        {
            if(remove)
                Remove();

            Tree = tree;
            Tree.RootNodes.AddLast(this);
        }
        
        /// <summary>
        /// Перемещает текущий TreeNode в указанный родительский узел
        /// </summary>
        public void AppendTo(TreeNode<T> parent, bool remove = true)
        {
            if(remove)
                Remove();

            Tree = parent.Tree;
            Parent = parent; 
            parent.Children.AddLast(this);
        }

        /// <summary>
        /// Удаляет связь с родителем
        /// </summary>
        public void Remove()
        {
            if (Parent?.Children.Remove(this) ?? false)
                Parent = null;
            else if (Tree?.RootNodes.Remove(this) ?? false)
                Tree = null!;
        }

        /// <summary>
        /// Проверяет, является ли текущий узел предком searchParent-узла 
        /// </summary>
        public bool IsChildOf(TreeNode<T> searchParent)
        {
            var parent = Parent;
            while (parent != null)
            {
                if (parent == searchParent)
                    return true;

                parent = parent.Parent;
            }

            return false;
        }

        #region DISPOSE

        /// <summary>
        /// Флаг окончания работы.
        /// </summary>
        protected bool IsDisposed;
        protected virtual void Dispose(bool disposing, bool sayGoodbye = true)
        {
            if (IsDisposed) 
                return;
            
            if (disposing)
            {
                Tag = null;
                Tree = null;
                Parent = null;
                foreach (var child in Children)
                    child.Dispose();
                Children.Clear();
            }
            IsDisposed = true;
        }

        /// <summary>
        /// Методы вызова очистки данных класса с оповещением сервера 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}