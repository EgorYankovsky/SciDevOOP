namespace SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;

/// <summary>
/// ���������������� ���������.
/// </summary>
public class ContinuousImprovement : ITransitionRule
{
    double ITransitionRule.Value(double currentTemperature, double newFunctionalValue, double minFunctionalValue)
        => newFunctionalValue < minFunctionalValue ? 1.0 : 0;
    
}