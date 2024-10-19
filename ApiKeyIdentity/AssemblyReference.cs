using System.Reflection;

namespace ApiKeyIdentity;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}