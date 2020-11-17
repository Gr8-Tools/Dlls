using System;
using AsyncClientServer.Attributes;
using AsyncClientServer.Message.Types.Utils;

namespace AsyncClientServer.Utils.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Получает из типа сообщения тип, кому предназанчено это сообщение
        /// </summary>
        public static MessageDestinationTypes GetMessageDestination(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (MessageDestination) Attribute.GetCustomAttribute(fieldInfo, typeof(MessageDestination));

            return descriptionAttribute?.Type ?? MessageDestinationTypes.Null;
        }

        /// <summary>
        /// Данный тип сообщения поддерживается сервером  
        /// </summary>
        public static bool IsServerSupported(this MessageDestinationTypes type)
            => type == MessageDestinationTypes.Server || type == MessageDestinationTypes.Both;

        /// <summary>
        /// Данный тип сообщения поддерживается клиентом  
        /// </summary>
        public static bool IsClientSupported(this MessageDestinationTypes type)
            => type == MessageDestinationTypes.Client || type == MessageDestinationTypes.Both;

        /// <summary>
        /// Генерирует инфомарцию о типе сообщения из перечисления
        /// </summary>
        public static MessageTypeInfo GenerateMessageInfo(this Enum value, byte index = 0)
        {
            if (index == 0)
                index = Convert.ToByte(value);
            
            return new MessageTypeInfo(value, index);
        }
    }
}