using System;

namespace ExtensionsLibrary
{
    public static partial class TypeExtensions
    {
        public static string GetName(this Type type, bool fullName = false)
        {
            if(fullName)
                return type.FullName?.Replace('+', '.') ?? type.Name;
            return type.Name;
        }
    }
}