using System.Reflection;

namespace WindowsServiceTools;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}