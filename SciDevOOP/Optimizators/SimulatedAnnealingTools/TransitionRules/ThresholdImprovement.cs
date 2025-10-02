using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;

/// <summary>
/// Пороговое улучшение.
/// </summary>
public class ThresholdImprovement(double currentTemperature, double alpha = 1.0) : ITransitionRule
{
    private readonly double _currentTemperature = currentTemperature;
    private readonly double _alpha = alpha;

    double ITransitionRule.Value(IFunctional functional, IParametricFunction function, IVector newParameters, IVector minParameters)
    {
        var newFunction = function.Bind(newParameters);
        var minFunction = function.Bind(minParameters);

        var currentFunctionalDifference = functional.Value(newFunction);
        var minimalFunctionalDifference = functional.Value(minFunction);
        return currentFunctionalDifference - minimalFunctionalDifference < _alpha * _currentTemperature ? 1 : 0;
    }
}