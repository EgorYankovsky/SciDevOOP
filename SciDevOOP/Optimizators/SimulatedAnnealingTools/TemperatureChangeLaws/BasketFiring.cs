using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;

/// <summary>
/// Отжиг Коши.
/// </summary>
public class BasketFiring : ITemperatureChangeLaw
{
    public double Value(double startTemperature, uint currentIteration)
    {
        /* density function */
        return startTemperature / currentIteration;
    }
}