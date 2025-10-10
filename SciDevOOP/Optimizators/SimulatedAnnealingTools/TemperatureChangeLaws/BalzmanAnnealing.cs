namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;

/// <summary>
/// �������������� �����.
/// </summary>
public class BalzmanAnnealing : ITemperatureChangeLaw
{
    public double Value(double startTemperature, uint currentIteration)
        => startTemperature / Math.Log(1 + currentIteration);
}