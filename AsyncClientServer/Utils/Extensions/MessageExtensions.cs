﻿using System;
using AsyncClientServer.Message;

namespace AsyncClientServer.Utils.Extensions
{
    public static class MessageExtensions
    {
        /// <summary>
        /// Создает заголовок сообщения, фиксируя в нем длину сообщения
        /// </summary>
        public static void CreateMessage(this IMessage message, int dataLength, out byte[] data)
        {
            dataLength += Params.HEADER_LENGTH;
            data = new byte[dataLength];
            SetBytes(in data, dataLength);
        }

        /// <summary>
        /// Возвращает занчение типа UInt16 из массива байтов начиная с offset
        /// </summary>
        public static ushort GetUInt16(in byte[] source, ref int offset)
        {
            const int length = sizeof(ushort);
            var value = BitConverter.ToUInt16(source, offset);
            offset += length;
            return value;
        }

        /// <summary>
        /// Возвращает занчение типа UInt16 из массива байтов начиная с offset
        /// </summary>
        public static short GetInt16(in byte[] source, ref int offset)
        {
            const int length = sizeof(short);
            var value = BitConverter.ToInt16(source, offset);
            offset += length;
            return value;
        }

        /// <summary>
        /// Возвращает занчение типа UInt16 из массива байтов начиная с offset
        /// </summary>
        public static bool GetBoolean(in byte[] source, ref int offset)
        {
            const int length = sizeof(bool);
            var value = BitConverter.ToBoolean(source, offset);
            offset += length;
            return value;
        }

        /// <summary>
        /// Возвращает массив байтов начиная с offset, заканичая либо length, либо последним байтом 
        /// </summary>
        public static byte[] GetBytes(in Array source, ref int offset, int length = 0)
        {
            if (length == 0)
                length = source.Length - offset;

            var result = new byte[length];
            Array.Copy(source, offset, result, 0, length);
            offset += length;
            return result;
        }

        /// <summary>
        /// Добавляет в массив байтов значение Value типа byte и возвращает новую позицию головки записи
        /// </summary>
        public static int SetByte(in byte[] destination, byte value, int offset = 0)
        {
            destination[offset] = value;
            return offset + 1;
        }

        /// <summary>
        /// Добавляет в массив байтов массив байтов Source и возвращает новую позицию головки записи
        /// </summary>
        public static int SetBytes(in byte[] destination, Array source, int offset = 0)
        {
            var length = source.Length;
            Array.Copy(source, 0, destination, offset, length);
            return offset + length;
        }

        /// <summary>
        /// Добавляет в массив байтов значение Value типа UInt16 и возвращает новую позицию головки записи
        /// </summary>
        public static int SetBytes(in byte[] destination, ushort value, int offset = 0, int length = 0)
        {
            if (length == 0)
                length = sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(value), 0, destination, offset, length);
            return offset + length;
        }
        
        /// <summary>
        /// Добавляет в массив байтов значение Value типа Int16 и возвращает новую позицию головки записи
        /// </summary>
        public static int SetBytes(in byte[] destination, short value, int offset = 0, int length = 0)
        {
            if (length == 0)
                length = sizeof(short);
            Array.Copy(BitConverter.GetBytes(value), 0, destination, offset, length);
            return offset + length;
        }

        /// <summary>
        /// Добавляет в массив байтов значение Value типа Int32 и возвращает новую позицию головки записи
        /// </summary>
        public static int SetBytes(in byte[] destination, int value, int offset = 0, int length = 0)
        {
            if (length == 0)
                length = sizeof(int);
            Array.Copy(BitConverter.GetBytes(value), 0, destination, offset, length);
            return offset + length;
        }

        /// <summary>
        /// Добавляет в массив байтов значение Value типа Bool и возвращает новую позицию головки записи
        /// </summary>
        public static int SetBytes(in byte[] destination, bool value, int offset = 0, int length = 0)
        {
            if (length == 0)
                length = sizeof(bool);
            Array.Copy(BitConverter.GetBytes(value), 0, destination, offset, length);
            return offset + length;
        }
    }
}