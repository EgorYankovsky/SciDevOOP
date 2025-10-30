namespace SciDevOOP.Optimizators.SimulatedAnnealingTools;

interface ITransitionRule
{
    /// <summary>
    /// Method, that calculates new step switch probability.
    /// </summary>
    /// <param name="currentTemperature">Current temperature at i-th iteration.</param>
    /// <param name="newFunctionalValue">Current functional's value.</param>
    /// <param name="minFunctionalValue">Minimal functional's value.</param>
    /// <returns>Probability of new step switch.</returns>
    public double Value(double currentTemperature, double newFunctionalValue, double minFunctionalValue);
}