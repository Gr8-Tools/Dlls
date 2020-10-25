using System;
using JetBrains.Annotations;

namespace UnityAsyncHelper.Wrappers
{
    /// <summary>
    /// Оболочка асинхронного выполнения задачи, где функция задачи принимает аргументы, а callback-функция - нет
    /// </summary>
    public class ParametriseThreadWrapper: ThreadWrapperBase
    {
        private readonly Action<object[]> _asyncAction;
        private readonly Action _callbackAction;
        private readonly object[] _args;
        
        public ParametriseThreadWrapper(Action<object[]> asyncAction, [CanBeNull] Action callbackAction, 
            object[] args, bool sendOnCompletedToMainThread) : base(sendOnCompletedToMainThread)
        {
            _asyncAction = asyncAction;
            _callbackAction = callbackAction;

            _args = args;
        }

        /// <summary>
        /// Функция выполнения задачи
        /// </summary>
        protected override void DoTask()
        {
            _asyncAction.Invoke(_args);
        }

        /// <summary>
        /// Функция оповещения окончания выполнения задачи
        /// </summary>
        protected override void OnCompleted()
        {
            _callbackAction?.Invoke();
        }
    }
}