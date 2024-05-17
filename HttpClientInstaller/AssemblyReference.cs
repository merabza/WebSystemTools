using System.Reflection;

namespace HttpClientInstaller;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}