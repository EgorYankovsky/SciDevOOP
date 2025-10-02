using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;

/// <summary>
/// Бальцмановский отжиг.
/// </summary>
public class ThresholdImprovement : ITemperatureChangeLaw
{
    public double Value(double startTemperature, uint currentIteration)
    {
        /* density function */
        return startTemperature / Math.Log(1 + currentIteration);
    }
}