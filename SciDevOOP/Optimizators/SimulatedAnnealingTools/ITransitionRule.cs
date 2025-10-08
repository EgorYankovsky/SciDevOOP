using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.SimulatedAnnealingTools;

interface ITransitionRule
{
    public double Value(double currentTemperature, double newFunctionalValue, double minFunctionalValue);
}