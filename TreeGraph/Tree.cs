using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeGraph
{
    public class Tree<T>
    {
        public readonly LinkedList<TreeNode<T>> RootNodes = new LinkedList<TreeNode<T>>();
        private readonly Dictionary<string, List<TreeNode<T>>> _tagMap = new Dictionary<string, List<TreeNode<T>>>();

        public void Clear()
        {
            foreach (var root in RootNodes)
                root.Dispose();
            
            RootNodes.Clear();
            _tagMap.Clear();
        }
        
        public void AddTag(string? tag, TreeNode<T> node)
        {
            if(tag == null)
                throw new Exception($"Can't add node \"{node.Value}\" with tag = null");
            
            if(!_tagMap.ContainsKey(tag))
                _tagMap.Add(tag, new List<TreeNode<T>>());
            
            if(!_tagMap[tag].Contains(node))
                _tagMap[tag].Add(node);
        }

        public void RemoveTag(string? tag, TreeNode<T> node)
        {
            if(tag == null)
                return;
            
            if(!_tagMap.ContainsKey(tag))
                return;

            _tagMap[tag].Remove(node);
            if (_tagMap[tag].Count == 0)
                _tagMap.Remove(tag);
        }

        public IEnumerable<TreeNode<T>> FindNodesByTag(string? tag)
        {
            if (tag == null || !_tagMap.ContainsKey(tag))
                return new TreeNode<T>[0];
            
            return _tagMap[tag];
        }

        public TreeNode<T>? FindNodeByTag(string? tag)
        {
            if (tag == null)
                return null;
            
            if (!_tagMap.ContainsKey(tag))
                return null;

            return _tagMap[tag].FirstOrDefault();
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
                Clear();
            
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