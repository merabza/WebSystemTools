using System.Reflection;

namespace ApiExceptionHandler;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}