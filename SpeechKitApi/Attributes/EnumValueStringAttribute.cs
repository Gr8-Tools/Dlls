﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace SpeechKitApi.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValueStringAttribute : Attribute
    {
        public string Name { get; }

        public EnumValueStringAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            Name = name;
        }
    }
}
