using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WebSystemTools.WindowsServiceTools.Tests;

//ტესტის პროცესის ProcessPowerThrottling-ის მდგომარეობა, შესამოწმებელი კოდისგან დამოუკიდებლად: GetProcessInformation-ით
//იკითხება, SetProcessInformation-ით კი EcoQoS ირთვება (Task Manager-ის efficiency mode-ის მსგავსად)
internal static class NativeMethods
{
    private const int ProcessPowerThrottling = 4;
    private const uint ProcessPowerThrottlingCurrentVersion = 1;

    public const uint ProcessPowerThrottlingExecutionSpeed = 0x1;

    [SupportedOSPlatform("windows")]
    public static (uint ControlMask, uint StateMask) GetPowerThrottling()
    {
        var state = new ProcessPowerThrottlingState { Version = ProcessPowerThrottlingCurrentVersion };
        Assert.True(GetProcessInformation(GetCurrentProcess(), ProcessPowerThrottling, ref state,
            Marshal.SizeOf<ProcessPowerThrottlingState>()));
        return (state.ControlMask, state.StateMask);
    }

    [SupportedOSPlatform("windows")]
    public static void TurnOnEfficiencyMode()
    {
        var state = new ProcessPowerThrottlingState
        {
            Version = ProcessPowerThrottlingCurrentVersion,
            ControlMask = ProcessPowerThrottlingExecutionSpeed,
            StateMask = ProcessPowerThrottlingExecutionSpeed
        };
        Assert.True(SetProcessInformation(GetCurrentProcess(), ProcessPowerThrottling, ref state,
            Marshal.SizeOf<ProcessPowerThrottlingState>()));
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetProcessInformation(nint hProcess, int processInformationClass,
        ref ProcessPowerThrottlingState processInformation, int processInformationSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessInformation(nint hProcess, int processInformationClass,
        ref ProcessPowerThrottlingState processInformation, int processInformationSize);

    [DllImport("kernel32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern nint GetCurrentProcess();

    [StructLayout(LayoutKind.Sequential)]
    private struct ProcessPowerThrottlingState
    {
        public uint Version;
        public uint ControlMask;
        public uint StateMask;
    }
}
