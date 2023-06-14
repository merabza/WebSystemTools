using System.Reflection;

namespace BackgroundTasksTools;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}