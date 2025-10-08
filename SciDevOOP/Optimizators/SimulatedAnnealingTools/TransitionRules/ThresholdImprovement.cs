namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;

/// <summary>
/// Пороговое улучшение.
/// </summary>
public class ThresholdImprovement(double currentTemperature, double alpha = 1.0) : ITransitionRule
{
    private readonly double _currentTemperature = currentTemperature;
    private readonly double _alpha = alpha;

    double ITransitionRule.Value(double currentTemperature, double newFunctionalValue, double minFunctionalValue)
        => newFunctionalValue - minFunctionalValue < _alpha * _currentTemperature ? 1.0 : 0.0;
}