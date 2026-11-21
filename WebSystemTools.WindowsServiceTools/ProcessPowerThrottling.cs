using System;
using Serilog;

namespace WebSystemTools.WindowsServiceTools;

//Windows-ის power throttling-ი (EcoQoS) ფონურ პროცესს ზღუდავს: ჰიბრიდულ პროცესორზე მისი ნაკადები E-ბირთვებზე და
//დაბალ სიხშირეზე გადადის. ასე ProGrammarGe-ის ანალიზი ≈ 3.2 ბირთვით შემოიფარგლებოდა, და ProcessorWorkerService-ის
//ნაკადური ანალიზი ≈ 3-ჯერ ნელი იყო. მეთოდი პროცესს ამ მექანიზმს უთიშავს (HighQoS), და Windows მას ფონურადაც აღარ ზღუდავს
// ReSharper disable once UnusedType.Global
public static class ProcessPowerThrottling
{
    public static bool DisableOnWindows(ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(DisableOnWindows));

        //false: Windows არ არის, ან Windows 10 1709-მდეა, სადაც ProcessPowerThrottling არ არსებობს და შეზღუდვაც არ არის
        bool disabled = OperatingSystem.IsWindows() && NativeMethods.DisableExecutionSpeedThrottling();

        debugLogger?.Information("{MethodName} Finished, disabled: {Disabled}", nameof(DisableOnWindows), disabled);

        return disabled;
    }
}
