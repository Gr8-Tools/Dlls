using System;
using JetBrains.Annotations;

namespace UnityAsyncHelper.Wrappers
{
    /// <summary>
    /// Оболочка асинхронного выполнения задачи, где функции задачи и callback не принимают параметры
    /// </summary>
    public class SimpleTreadWrapper: ThreadWrapperBase
    {
        private readonly Action _asyncAction;
        private readonly Action _callbackAction;

        public SimpleTreadWrapper(Action asyncAction, [CanBeNull] Action callbackAction, bool sendOnCompletedToMainThread) : base(sendOnCompletedToMainThread)
        {
            _asyncAction = asyncAction;
            _callbackAction = callbackAction;
        }
        
        /// <summary>
        /// Функция выполнения задачи
        /// </summary>
        protected override void DoTask()
        {
            _asyncAction.Invoke();
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