using System;
using JetBrains.Annotations;

namespace UnityAsyncHelper.Wrappers
{
    /// <summary>
    /// Оболочка асинхронного выполнения задачи, где функция задачи выдает результат, необходимый для callBack-функции
    /// </summary>
    public class FunctionalThreadWrapper: ThreadWrapperBase
    {
        private readonly Func<object[]> _asyncAction;
        private readonly Action<object[]> _callbackAction;
        private object[] _results;
        
        public FunctionalThreadWrapper(Func<object[]> asyncAction, [CanBeNull] Action<object[]> callbackAction, bool sendOnCompletedToMainThread) : base(sendOnCompletedToMainThread)
        {
            _asyncAction = asyncAction;
            _callbackAction = callbackAction;
        }

        /// <summary>
        /// Функция выполнения задачи
        /// </summary>
        protected override void DoTask()
        {
            _results = _asyncAction.Invoke();
        }

        /// <summary>
        /// Функция оповещения окончания выполнения задачи
        /// </summary>
        protected override void OnCompleted()
        {
            _callbackAction?.Invoke(_results);
        }
    }
}