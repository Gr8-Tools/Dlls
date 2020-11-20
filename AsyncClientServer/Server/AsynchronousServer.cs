using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AsyncClientServer.Client;
using AsyncClientServer.Utils;
using UniLog;
using UnityAsyncHelper.Core;

namespace AsyncClientServer.Server
{
    /// <summary>
    /// Асинхронный мультипоточный сервер
    /// </summary>
    public class AsynchronousServer: IDisposable
    {
        public event Action<object[]> onConnected = objects => { }; 
        
        public static readonly Dictionary<AsynchronousServer, List<AsynchronousClient>> ListenerClientMap = new Dictionary<AsynchronousServer, List<AsynchronousClient>>();

        private readonly ManualResetEvent _allDone = new ManualResetEvent(false);
        private readonly Socket _socket;
        private readonly bool _raiseCallbacksOnMainThread;

        /// <summary>
        /// Конструктор класса сервера
        /// </summary>
        private AsynchronousServer(IPEndPoint localEndPoint, bool raiseCallbacksOnMainThread)
        {
            _socket = new Socket(localEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _raiseCallbacksOnMainThread = raiseCallbacksOnMainThread;
        }
        
        /// <summary>
        /// Начинает прослшку конкретного узла
        /// </summary>
        public static void StartListening(IPEndPoint localEndPoint, out AsynchronousServer listener, bool raiseCallbacksOnMainThread)
        {
            try
            {
                listener = ListenerClientMap.Keys.FirstOrDefault(l =>
                    ((IPEndPoint) l._socket.LocalEndPoint).Equals(localEndPoint));
                if (listener != null)
                {
                    Logger.ShowMessage($"Listener of {localEndPoint} is already opened");
                    return;
                }

                listener = new AsynchronousServer(localEndPoint, raiseCallbacksOnMainThread);
                listener._socket.Bind(localEndPoint);
                listener._socket.Listen(Params.SERVER_CONNECTIONS_COUNT);
                ListenerClientMap.Add(listener, new List<AsynchronousClient>());

                ThreadManager.AsyncExecute(listener.AcceptClients, null);
            }
            catch (Exception e)
            {
                listener = null;
                Logger.ShowException(e);
            }
        }

        /// <summary>
        /// Принимает новые входящие подключения. Запускаем в новом потоке.
        /// </summary>
        private void AcceptClients()
        {
            try
            {
                while (!IsDisposed)
                {
                    Logger.ShowMessage("Waiting for connection...");

                    _allDone.Reset();
                    _socket.BeginAccept(AcceptCallback, this);
                    _allDone.WaitOne();
                }

                Logger.ShowMessage($"Server listening point {_socket.LocalEndPoint} is closed");
            }
            catch (Exception e)
            {
                Logger.ShowException(e);
            }
        }

        /// <summary>
        /// Фиксирует новое входящее подключение.
        /// Подключенный источник сохраняется в карту.
        /// Запсукается ожидание чтения.
        /// </summary>
        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var server = (AsynchronousServer) ar.AsyncState;
                server._allDone.Set();
                var client = new AsynchronousClient(server._socket.EndAccept(ar), false, server._raiseCallbacksOnMainThread);
                ListenerClientMap[server].Add(client);

                client.Receive();
                server.RaiseEvent(server.onConnected, client);
            }
            catch (Exception e)
            {
                Logger.ShowException(e);
            }
        }
        
        /// <summary>
        /// Поднимает событие
        /// </summary>
        private void RaiseEvent(Action<object[]> action, params object[] args)
        {
            if (_raiseCallbacksOnMainThread)
                ThreadManager.ExecuteOnMainThread(() => action.Invoke(args));
            else
                action?.Invoke(args);
        }

        #region DISPOSE

        /// <summary>
        /// Флаг окончания работы.
        /// </summary>
        public bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    foreach (var client in ListenerClientMap[this])
                    {
                        client.Dispose();
                    }
                    ListenerClientMap[this].Clear();
                    ListenerClientMap.Remove(this);
                }
                IsDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}