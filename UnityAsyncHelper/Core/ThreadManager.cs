using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityAsyncHelper.Wrappers;
using UnityEngine;

namespace UnityAsyncHelper.Core
{
    /// <summary>
    /// Контроллер потоков (позволяет выполнять вычисления в других потоках и обрабатывать резултат в базовом)
    /// </summary>
    public class ThreadManager : MonoBehaviour
    {
        public delegate object[] ReturnAction(); 
        
        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool _actionToExecuteOnMainThread = false;

        private void Update()
        {
            UpdateMain();
        }

        /// <summary>
        /// Запускает действие в новом потоке и выполняет Callback-функцию в базовом 
        /// </summary>
        public static void AsyncExecute(Action asyncAction, [CanBeNull] Action callback)
        {
            var simpleThreadWrapper = new SimpleTreadWrapper(asyncAction, callback, true);
            simpleThreadWrapper.Start();
        }
        
        /// <summary>
        /// Запускает действие в новом потоке c переданными параметрами и выполняет Callback-функцию в базовом 
        /// </summary>
        public static void AsyncExecute(Action<object[]> asyncAction, [CanBeNull] Action callback, params object[] invokeArgs)
        {
            var threadWrapper = new ParametriseThreadWrapper(asyncAction, callback, invokeArgs, true);
            threadWrapper.Start();
        }
        
        /// <summary>
        /// Запускает действие в новом потоке, возвращает результат и выполняет Callback-функцию с рещультом в базовом потоке 
        /// </summary>
        public static void AsyncExecute(Func<object[]> asyncAction, [CanBeNull] Action<object[]> callback)
        {
            var threadWrapper = new FunctionalThreadWrapper(asyncAction, callback, true);
            threadWrapper.Start();
        }

        /// <summary>
        /// Добавляет в очередь новое действие, для выполнения в основном потоке 
        /// </summary>
        public static void ExecuteOnMainThread([CanBeNull] Action action)
        {
            if (action == null)
            {
                Debug.Log("No action to execute on main thread!");
                return;
            }

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(action);
                _actionToExecuteOnMainThread = true;
            }
        }
        
        
        /// <summary>
        /// Выполняет в главном потоке все зарегистрированные действия
        /// </summary>
        private void UpdateMain()
        {
            if(!_actionToExecuteOnMainThread)
                return;

            lock (executeOnMainThread)
            {
                executeCopiedOnMainThread.AddRange(executeOnMainThread);
                executeOnMainThread.Clear();
                _actionToExecuteOnMainThread = false;
            }

            foreach (var action in executeCopiedOnMainThread)
                action.Invoke();
            
            executeCopiedOnMainThread.Clear();
        }
    }
}