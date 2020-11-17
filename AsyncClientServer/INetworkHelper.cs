using System;
using AsyncClientServer.Message;

namespace AsyncClientServer
{
    public interface INetworkHelper
    {
        /// <summary>
        /// Функция десериализации сообщений (всех дополнительных типов)
        /// </summary>
        Func<byte, byte[], IMessage> DeserializationFunction { get; }
        
        /// <summary>
        /// Добавляет в список все необходимые наборы информации о сообщениях
        /// </summary>
        void SetUpMessageTypeInfos();
    }
}