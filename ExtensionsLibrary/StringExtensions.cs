using System;

namespace ExtensionsLibrary
{
    public static partial class StringExtensions
    {
        private static readonly string[] RowSeparator = new[]
        {
            Environment.NewLine
        };
        
        /// <summary>
        /// Returns string array from string, separated by lines 
        /// </summary>
        public static string[] GetRows(this string value)
            => value.Split(RowSeparator, StringSplitOptions.None);

        /// <summary>
        /// Returns string value with firstChar lower 
        /// </summary>
        public static string ToLowerFirstChar(this string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new Exception($"String \"{value}\" is null or whiteSpace");
            
            if (char.IsUpper(value[0]))
                value =  char.ToLower(value[0]) + value.Substring(1);

            return value;
        }
    }
}