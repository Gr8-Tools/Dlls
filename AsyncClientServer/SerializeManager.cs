using AsyncClientServer.Message;

namespace AsyncClientServer
{
    /// <summary>
    /// Менеджер сериализации/десериализации сообщение сообщений.
    /// Пробрасывает десериализацию на статичный метод нужного класса, ориентируясь на тип сообщения
    /// </summary>
    public static class SerializeManager
    {
        /// <summary>
        /// Функция, возвращающая объект сообщения из массива байтов, переопределяемая в методе SetUp 
        /// </summary>
        private static INetworkHelper _networkHelper;

        /// <summary>
        /// Устанавливает функцию конвертации массива байтов в объект сообщения
        /// Используется в реализации для конвертации конкретных типов 
        /// </summary>
        public static void SetUp(INetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
        }
        
        /// <summary>
        /// Возвращает объект сообщения из массива байтов
        /// </summary>
        public static IMessage Deserialise(byte messageTypeIndex, in byte[] data)
        {
            switch (messageTypeIndex)
            {
                case 0:
                    return null;
                case 1: //MessageType.CloseConnection
                    return CloseConnectionMessage.Deserialize(in data);
                default:
                    return _networkHelper?.DeserializationFunction(messageTypeIndex, data);
            }
        }
    }
}





















