using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.SimulatedAnnealingTools;

interface ITemperatureChangeLaw
{
    /// <summary>
    /// Method, that calculates new temperature after algorithm's iteration. 
    /// </summary>
    /// <param name="startTemperature">First temperature.</param>
    /// <param name="currentIteration">Current iteration.</param>
    /// <returns>Current temperature.</returns>
    public double Value(double startTemperature, uint currentIteration);
}