namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;

public class ThresholdImprovement(double alpha = 0.1) : ITransitionRule
{
    private readonly double _alpha = alpha;

    double ITransitionRule.Value(double currentTemperature, double newFunctionalValue, double minFunctionalValue)
        => newFunctionalValue - minFunctionalValue < _alpha * currentTemperature ? 1.0 : 0.0;
}