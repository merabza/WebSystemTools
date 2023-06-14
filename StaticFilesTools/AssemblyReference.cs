using System.Reflection;

namespace StaticFilesTools;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}