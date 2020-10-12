using System;
using TreeModule.Utils;

namespace TreeModule
{
    public class Tree<T> : IDisposable
    {
        public readonly TrackingDirection DefaultTrackingDirection = TrackingDirection.ToBrother;
        
        public readonly TreeRootNode<T> Root;
        public TreeRootNode<T> Current;
        
        public Tree()
        {
            Root = new TreeRootNode<T>(this).SetCurrent();
        }

        public Tree(TrackingDirection trackingDirection)
        {
            Root = new TreeRootNode<T>(this).SetCurrent();
            DefaultTrackingDirection = trackingDirection;
        }

        public Tree(T value)
        {
            Root = new TreeRootNode<T>(this, value).SetCurrent();
        }
        
        public Tree(T value, TrackingDirection trackingDirection)
        {
            Root = new TreeRootNode<T>(this, value).SetCurrent();
            DefaultTrackingDirection = trackingDirection;
        }
        
        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) 
                return;
            
            if (disposing)
            {
                if(!Root.DisposedValue)
                    Root.Dispose();
            }
            _disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}