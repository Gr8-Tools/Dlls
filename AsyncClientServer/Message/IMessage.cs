using AsyncClientServer.Message.Types.Utils;

namespace AsyncClientServer.Message
{
    public interface IMessage
    {
        /// <summary>
        /// Тип сообщения
        /// </summary>
        MessageTypeInfo MessageTypeInfo { get; }

        /// <summary>
        /// Функция сериализации сообщения в массив байтов 
        /// </summary>
        byte[] Serialize();
    }
}