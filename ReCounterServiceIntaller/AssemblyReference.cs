using System.Reflection;

namespace ReCounterServiceInstaller;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}