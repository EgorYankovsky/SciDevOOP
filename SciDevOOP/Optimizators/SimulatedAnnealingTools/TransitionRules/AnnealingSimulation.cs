namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;

/// <summary>
/// Имитация отжига.
/// </summary>
public class AnnealingSimulation(double alpha = 0.1) : ITransitionRule
{
    private readonly double Alpha = alpha;

    double ITransitionRule.Value(double currentTemperature, double newFunctionalValue, double minFunctionalValue)
        => newFunctionalValue < minFunctionalValue ? 1.0 : Math.Exp(-(newFunctionalValue - minFunctionalValue) / (currentTemperature * Alpha));
}