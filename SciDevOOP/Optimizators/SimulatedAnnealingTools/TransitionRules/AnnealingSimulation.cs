namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;

/// <summary>
/// Имитация отжига.
/// </summary>
public class AnnealingSimulation : ITransitionRule
{
    public double Alpha = 0.01;

    double ITransitionRule.Value(double currentTemperature, double newFunctionalValue, double minFunctionalValue)
        => newFunctionalValue < minFunctionalValue ? 1.0 : Math.Exp((newFunctionalValue - minFunctionalValue) / (currentTemperature * Alpha));
}