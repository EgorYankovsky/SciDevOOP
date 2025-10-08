using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;

/// <summary>
/// Последовательное улучшение.
/// </summary>
public class ContinuousImprovement : ITransitionRule
{
    double ITransitionRule.Value(double currentTemperature, IFunctional functional, IParametricFunction function, IVector newParameters, IVector minParameters)
    {
        var newFunction = function.Bind(newParameters);
        var minFunction = function.Bind(minParameters);

        var currentFunctionalDifference = functional.Value(newFunction);
        var minimalFunctionalDifference = functional.Value(minFunction);

        return currentFunctionalDifference < minimalFunctionalDifference ? 1.0 : 0;
    }
}