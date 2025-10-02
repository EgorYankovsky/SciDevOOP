using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.SimulatedAnnealingTools;

interface ITemperatureChangeLaw
{
    public double Value(double startTemperature, uint currentIteration);
}