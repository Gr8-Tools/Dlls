using System;
using Swipe.Extensions;
using UnityEngine;

namespace Swipe.Utils.Input
{
    /// <summary>
    /// Информация о сетке считывания свайпов
    /// </summary>
    [Serializable]
    public class InputGrid
    {
        /// <summary>
        /// Разрешение экрана
        /// </summary>
        private static Resolution ScreeSize
        {
            get =>
                _screeSize.IsEmpty()
                    ? _screeSize = new Resolution { width = Screen.width, height = Screen.height }
                    : _screeSize;
            set => _screeSize = value;
        }
        private static Resolution _screeSize = new Resolution { width = 0, height = 0, refreshRate = 0 };

        /// <summary>
        /// Количество строк сетки
        /// </summary>
        public uint Rows { 
            get => rows;
            private set => rows = value;
        }
        
        /// <summary>
        /// Количество колонок сетки
        /// </summary>
        public uint Columns { 
            get => columns;
            private set => columns = value;
        }

        [SerializeField] private uint rows;
        [SerializeField] private uint columns;

        public InputGrid(uint rows, uint columns)
        {
            Rows = rows;
            Columns = columns;
        }

        /// <summary>
        /// Возвращает УИД ячейки сетки по ее строке и колонке 
        /// </summary>
        public uint GetCellId(uint row, uint col)
        {
            return row * Columns + col;
        }
        
        /// <summary>
        /// Возвращает УИД ячейки сетки по переданным координатам 
        /// </summary>
        public uint GetCellId(Vector2 position)
        {
            if (Rows == 0 || Columns == 0)
                throw new System.Exception($"Input Grid is not filled! [{Rows};{Columns}]");

            //Так как позиция отсчитывается от левого нижнего угла экрана, а считать привычнеее справа-налоево, сверху-вниз, то номер строки должен быть реверснутым
            var currentRow = (Rows -1)-(uint)(position.y / (ScreeSize.height / Rows)); 
            var currentColumn = (uint)(position.x / (ScreeSize.width / Columns));

            return currentRow * Columns + currentColumn;
        }

        /// <summary>
        /// Возвращет размер сетки
        /// </summary>
        public uint[] Size => new[] { Rows, Columns };
    }
}