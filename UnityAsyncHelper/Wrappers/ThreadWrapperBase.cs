using System;
using System.Threading;
using UnityAsyncHelper.Core;
using UnityAsyncHelper.Utils;

namespace UnityAsyncHelper.Wrappers
{
    public abstract class ThreadWrapperBase
    {
        /// <summary>
        /// Состояние выполнения потока
        /// </summary>
        public ThreadStatus ThreadProgressStatus { get; protected set; } = ThreadStatus.UnStarted;
        
        /// <summary>
        /// Передать OnComplete функцию в основной поток
        /// </summary>
        private readonly bool _sendOnCompletedToMainThread;
        
        /// <summary>
        /// Поток выполнения задачи
        /// </summary>
        private Thread _thread;

        protected ThreadWrapperBase(bool sendOnCompletedToMainThread)
        {
            _sendOnCompletedToMainThread = sendOnCompletedToMainThread;
        }
        
        
        /// <summary>
        /// Запускает выполнение задачи в асинхронном потоке 
        /// </summary>
        public void Start()
        {
            if(ThreadProgressStatus > ThreadStatus.UnStarted)
                throw new InvalidOperationException("Task has been already execute");

            ThreadProgressStatus = ThreadStatus.InProgress;
            _thread = new Thread(StartTaskAsync)
            {
                IsBackground = true
            };

            _thread.Start();
        }

        /// <summary>
        /// Оберточкая функцния выполнения задачи
        /// </summary>
        private void StartTaskAsync()
        {
            DoTask();
            ThreadProgressStatus = ThreadStatus.Completed;
            if(_sendOnCompletedToMainThread)
                ThreadManager.ExecuteOnMainThread(OnCompleted);
            else
                OnCompleted();
        }

        /// <summary>
        /// Функция выполнения задачи
        /// </summary>
        protected abstract void DoTask();
        
        /// <summary>
        /// Функция оповещения окончания выполнения задачи
        /// </summary>
        protected abstract void OnCompleted();
    }
}