using System;
using System.Linq;

namespace SpeechKitApi.Utils
{
    public static class IoExtensions
    {
        public static string GetValidPathString(this string value)//, params char[] externalSymbols)
        {
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            // if (externalSymbols.Length > 0)
            // {
            //     Array.Resize(ref invalidChars, invalidChars.Length + externalSymbols.Length);
            //     externalSymbols.CopyTo(invalidChars, invalidChars.Length);
            // }
            
            return new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
        }
    }
}