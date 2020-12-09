using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGeneration.Utils
{
    public static class InheritorEntities
    {
        public static List<InheritorInfo<T>> GetInheritorsCollection<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(InheritorInfo<T>.Build)
                .ToList();
        }
    }
}