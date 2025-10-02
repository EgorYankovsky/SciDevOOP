using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.SimulatedAnnealingTools;

interface ITransitionRule
{
    public double Value(double currentTemperature, IFunctional functional, IParametricFunction function, IVector newParameters, IVector minParameters);
}