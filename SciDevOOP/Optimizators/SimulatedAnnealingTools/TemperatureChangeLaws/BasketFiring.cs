namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;

public class BasketFiring : ITemperatureChangeLaw
{
    public double Value(double startTemperature, uint currentIteration)
        => startTemperature / currentIteration;
}