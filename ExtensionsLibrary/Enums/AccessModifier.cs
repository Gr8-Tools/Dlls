using System.ComponentModel;

namespace Utils.Enums
{
    public enum AccessModifier
    {
        [Description("private")]
        Private, 
        [Description("protected")]
        Protected, 
        [Description("protected internal")]
        ProtectedInternal,
        [Description("internal")]
        Internal,  
        [Description("public")]
        Public
    }
}