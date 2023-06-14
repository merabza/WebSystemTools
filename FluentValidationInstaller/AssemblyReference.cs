using System.Reflection;

namespace FluentValidationInstaller;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}