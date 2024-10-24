using System.Reflection;

namespace SignalRRecounterMessages;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}