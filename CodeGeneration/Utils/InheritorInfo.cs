using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeGeneration.Utils
{
    public class InheritorInfo<T>
    {
        public string FilePath;
        public Type Type;
        
        public static IEnumerable<InheritorInfo<T>> Build(Assembly assembly)
        {
            var list = new List<InheritorInfo<T>>();
            var baseType = typeof(T);

            var types = assembly
                .GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) && t != baseType);

            if (!types.Any())
                return list;

            list.AddRange(
                types.Select(t => new InheritorInfo<T>
                {
                    Type = t, 
                    FilePath = Path.Combine(DefaultGenerationParams.ScriptPath, t.Namespace?.Replace('.', '\\') ?? "")
                }));
            return list;
        }
    }
}