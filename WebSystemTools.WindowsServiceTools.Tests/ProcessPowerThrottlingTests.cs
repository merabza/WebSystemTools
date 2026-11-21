using System;

namespace WebSystemTools.WindowsServiceTools.Tests;

//DisableOnWindows-ის შემდეგ პროცესი HighQoS-შია: ControlMask-ში EXECUTION_SPEED-ის ბიტია, StateMask-ში — არა.
//ორივე ტესტი ერთ კლასშია, ამიტომ მიმდევრობით სრულდება და ტესტის პროცესის მდგომარეობას ერთმანეთს არ უცვლის შუაში
public sealed class ProcessPowerThrottlingTests
{
    private const uint ExecutionSpeed = NativeMethods.ProcessPowerThrottlingExecutionSpeed;

    [Fact]
    public void DisableOnWindows_TurnsExecutionSpeedThrottlingOff()
    {
        // Act
        bool disabled = ProcessPowerThrottling.DisableOnWindows(null);

        // Assert
        Assert.Equal(OperatingSystem.IsWindows(), disabled);
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal((ExecutionSpeed, 0u), NativeMethods.GetPowerThrottling());
        }
    }

    //efficiency mode-ში მყოფი პროცესი (C4-ის ProGrammarGe-ის მდგომარეობის მსგავსი) ჩვეულებრივ რეჟიმზე გადადის
    [Fact]
    public void DisableOnWindows_EfficiencyModeIsOn_TurnsItOff()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.False(ProcessPowerThrottling.DisableOnWindows(null));
            return;
        }

        // Arrange
        NativeMethods.TurnOnEfficiencyMode();
        Assert.Equal((ExecutionSpeed, ExecutionSpeed), NativeMethods.GetPowerThrottling());

        // Act
        bool disabled = ProcessPowerThrottling.DisableOnWindows(null);

        // Assert
        Assert.True(disabled);
        Assert.Equal((ExecutionSpeed, 0u), NativeMethods.GetPowerThrottling());
    }
}
