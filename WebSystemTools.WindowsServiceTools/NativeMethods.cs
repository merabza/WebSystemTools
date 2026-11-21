using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WebSystemTools.WindowsServiceTools;

//kernel32-ის SetProcessInformation ProcessPowerThrottling კლასით (processthreadsapi.h, PROCESS_POWER_THROTTLING_STATE)
internal static class NativeMethods
{
    private const int ProcessPowerThrottling = 4;
    private const uint ProcessPowerThrottlingCurrentVersion = 1;
    private const uint ProcessPowerThrottlingExecutionSpeed = 0x1;

    //HighQoS: ControlMask-ში მექანიზმი პროცესის მართვაზე გადადის, StateMask-ში მისი ბიტის არქონა კი მას თიშავს
    [SupportedOSPlatform("windows")]
    internal static bool DisableExecutionSpeedThrottling()
    {
        var state = new ProcessPowerThrottlingState
        {
            Version = ProcessPowerThrottlingCurrentVersion,
            ControlMask = ProcessPowerThrottlingExecutionSpeed,
            StateMask = 0
        };
        return SetProcessInformation(GetCurrentProcess(), ProcessPowerThrottling, ref state,
            Marshal.SizeOf<ProcessPowerThrottlingState>());
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessInformation(nint hProcess, int processInformationClass,
        ref ProcessPowerThrottlingState processInformation, int processInformationSize);

    //ფსევდო-handle-ი, მისი დახურვა საჭირო არ არის
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
