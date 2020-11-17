using System;
using AsyncClientServer.Utils;

namespace AsyncClientServer.Attributes
{
    /// <summary>
    /// Аттрибут назначения сообщения
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MessageDestination: Attribute
    {
        public readonly MessageDestinationTypes Type;

        public MessageDestination(MessageDestinationTypes type)
        {
            Type = type;
        }
    }
}