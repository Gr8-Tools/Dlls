using System;
using Swipe.Utils.Send;

namespace Swipe.Utils.Special
{
    /// <summary>
    /// Дополнительное условие срабатывания свайпа
    /// </summary>
    [Serializable]
    public class Condition
    {
        public LengthCondition lengthCondition;

        public TimeCondition timeCondition;

        public GridCondition gridCondition;
        
        /// <summary>
        /// Выполняются ли условия свайпа 
        /// </summary>
        public bool Achieved(SwipeInfo swipeInfo)
        {
            if(lengthCondition != null && lengthCondition.IsUsed && !lengthCondition.Achieved(swipeInfo))
                return false;

            if (timeCondition != null && timeCondition.IsUsed && !timeCondition.Achieved(swipeInfo))
                return false;

            if (gridCondition != null && gridCondition.IsUsed && !gridCondition.Achieved(swipeInfo))
                return false;
            
            return true;
        }
    }
}