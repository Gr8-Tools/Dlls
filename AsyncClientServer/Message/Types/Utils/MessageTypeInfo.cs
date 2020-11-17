using System;

namespace AsyncClientServer.Message.Types.Utils
{
    /// <summary>
    /// Информация о типе сообщения 
    /// </summary>
    public class MessageTypeInfo
    {
        public readonly Enum Value;
        public readonly byte Index;

        public MessageTypeInfo(Enum value, byte index)
        {
            Value = value;
            Index = index;
        }
        
        /// <summary>
        /// Выполняет сравнение экземпляра класса с другим экземпляром, значением перечилсения или индекса
        /// </summary>
        public bool IsThis(object obj)
        {
            switch (obj)
            {
                case MessageTypeInfo typeInfo:
                    return Value.Equals(typeInfo.Value);
                case Enum enumValue:
                    return Value.Equals(enumValue);
                case byte index:
                    return Index == index;
                default:
                    return false;
            }
        }
    }
}