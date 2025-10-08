namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;

/// <summary>
/// ����� ����.
/// </summary>
public class BasketFiring : ITemperatureChangeLaw
{
    public double Value(double startTemperature, uint currentIteration)
        => startTemperature / currentIteration;
}