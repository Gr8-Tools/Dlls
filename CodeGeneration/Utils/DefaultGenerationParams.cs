using System.IO;
using UnityEngine;

namespace CodeGeneration.Utils
{
    public static class DefaultGenerationParams
    {
        public static readonly string ScriptPath = Path.Combine(
            Application.dataPath.Replace('/', '\\'), "Scripts");

        /// <summary>
        /// Returns FullFileName string 
        /// </summary>
        public static string GetFullFileName<T>(InheritorInfo<T> inheritorInfo)
            => $"{inheritorInfo.FilePath}\\{inheritorInfo.Type.Name}.cs";
    }
}