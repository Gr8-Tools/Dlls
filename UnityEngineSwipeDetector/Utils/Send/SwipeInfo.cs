using Swipe.Utils.Input;

namespace Swipe.Utils.Send
{
    /// <summary>
    /// Сущность "Свайпа"
    /// </summary>
    public class SwipeInfo
    {
        /// <summary>
        /// Размер сетки { Rows, Columns }
        /// </summary>
        public uint[] GridSize;
        
        /// <summary>
        /// Информация о точке нажатия
        /// </summary>
        public InputInfo StartInfo;
        
        /// <summary>
        /// Информация о точке отпускания
        /// </summary>
        public InputInfo FinishInfo;

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public OptionalSwipeInfo OptionalInfo { get; private set; }

        /// <summary>
        /// Формирует дополнитлеьные опции 
        /// </summary>
        public void GetOptionalInfo(bool useEightDirections)
        {
            OptionalInfo = new OptionalSwipeInfo(this, useEightDirections);
        }
    }
}