using System;
using System.Collections.Generic;
using System.Reflection;
using Utils.Enums;

namespace ExtensionsLibrary
{
    public static partial class ReflectionExtensions
    {
        public static readonly List<AccessModifier> AccessModifiers = new List<AccessModifier>
        {
            AccessModifier.Private, 
            AccessModifier.Protected, 
            AccessModifier.ProtectedInternal,
            AccessModifier.Internal,  
            AccessModifier.Public
        };

        public static AccessModifier GetAccessModifier(this FieldInfo fieldInfo)
        {
            if (fieldInfo.IsPrivate)
                return AccessModifier.Private;
            if (fieldInfo.IsFamily)
                return AccessModifier.Protected;
            if (fieldInfo.IsFamilyOrAssembly)
                return AccessModifier.ProtectedInternal;
            if (fieldInfo.IsAssembly)
                return AccessModifier.Internal;
            if (fieldInfo.IsPublic)
                return AccessModifier.Public;
            
            throw new ArgumentException("Did not find access modifier",nameof(fieldInfo));
        }
        
        public static AccessModifier GetAccessModifier(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod == null)
                return propertyInfo.GetMethod.GetAccessModifier();
            if (propertyInfo.GetMethod == null)
                return propertyInfo.SetMethod.GetAccessModifier();
            var max = Math.Max(AccessModifiers.IndexOf(propertyInfo.GetMethod.GetAccessModifier()),
                AccessModifiers.IndexOf(propertyInfo.SetMethod.GetAccessModifier()));
            return AccessModifiers[max];
        }
        
        public static AccessModifier GetAccessModifier(this MethodInfo methodInfo)
        {
            if (methodInfo.IsPrivate)
                return AccessModifier.Private;
            if (methodInfo.IsFamily)
                return AccessModifier.Protected;
            if (methodInfo.IsFamilyOrAssembly)
                return AccessModifier.ProtectedInternal;
            if (methodInfo.IsAssembly)
                return AccessModifier.Internal;
            if (methodInfo.IsPublic)
                return AccessModifier.Public;
            throw new ArgumentException("Did not find access modifier", nameof(methodInfo));
        }
    }
}