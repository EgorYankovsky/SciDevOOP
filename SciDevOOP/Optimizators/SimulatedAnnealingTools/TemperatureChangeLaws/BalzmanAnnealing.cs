namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;

/// <summary>
/// Бальцмановский отжиг.
/// </summary>
public class ThresholdImprovement : ITemperatureChangeLaw
{
    public double Value(double startTemperature, uint currentIteration)
        => startTemperature / Math.Log(1 + currentIteration);
}