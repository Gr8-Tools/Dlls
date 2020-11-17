using System;
using System.Collections.Generic;
using System.Linq;
using AsyncClientServer.Message.Types.Utils;
using AsyncClientServer.Utils.Extensions;
using UniLog;

namespace AsyncClientServer.Message.Types
{
    /// <summary>
    /// Менеджер работы с типами сообщений
    /// </summary>
    public static class MessageTypeManager
    {
        private static readonly List<MessageTypeInfo> _list = new List<MessageTypeInfo>(byte.MaxValue);

        /// <summary>
        /// Инициализация базовых значений
        /// </summary>
        public static void Initialize()
        {
            Add(typeof(DefaultMessageTypes));
        }
        
        /// <summary>
        /// Добавляет информацию о типе сообщения в список и возвращает результат добавления 
        /// </summary>
        public static bool Add(Enum enumValue)
        {
            var info = Get(enumValue);
            if (info == null)
            {
                if (_list.Count == _list.Capacity)
                {
                    Logger.ShowMessage("Message register has no memory!");
                    return false;
                }
                
                info = enumValue.GenerateMessageInfo((byte) _list.Count);
                _list.Add(info);
            }
            
            return true;
        }

        /// <summary>
        /// Добавялет информацию о типе сообщения в список по набору значений перечисления
        /// </summary>
        public static bool AddRange(params Enum[] values)
        {
            var result = true;
            foreach (var value in values)
                result &= Add(value);

            return result;
        }

        /// <summary>
        /// Добавляет информацию о типе сообщения в список по всем значениям типа перечисления 
        /// </summary>
        public static bool Add(Type enumType)
        {
            if (!enumType.IsEnum)
                return false;

            var result = true;
            foreach (var enumValue in Enum.GetValues(enumType))
                result &= Add((Enum) enumValue);

            return result;
        }

        /// <summary>
        /// Возвращает информацию о значении типа сообщения по числовому значению
        /// </summary>
        public static MessageTypeInfo Get(byte index)
        {
            return _list.Count > index ? _list[index] : null;
        }

        /// <summary>
        /// Возвращает информацию о значении типа сообщения по представлению
        /// </summary>
        public static MessageTypeInfo Get(Enum enumValue)
        {
            return _list.FirstOrDefault(v => v.IsThis(enumValue));
        }
    }
}