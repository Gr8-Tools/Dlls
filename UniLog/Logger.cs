using System;
using UniLog.Utils;

namespace UniLog
{
    /// <summary>
    /// Класс, перенаправляющий информацию лога от источника, к любому реализатору
    /// </summary>
    public static class Logger
    {
        public static event Action<string> onInfo = s => { };  
        public static event Action<string> onWarning = s => { };  
        public static event Action<string> onError = s => { };  
        
        /// <summary>
        /// Перенаправлет сообщение строкового типа 
        /// </summary>
        public static void ShowMessage(string message, MessageType type = MessageType.Info)
        {
            switch (type)
            {
                case MessageType.Info:
                    onInfo(message);
                    break;
                case MessageType.Warning:
                    onWarning(message);
                    break;
                case MessageType.Error:
                    onError(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        /// <summary>
        /// Перенаправляет сообщение-исключение 
        /// </summary>
        public static void ShowException(Exception ex, MessageType type = MessageType.Error)
        {
            ShowMessage(ex.ToString(), type);
        }
    }
}