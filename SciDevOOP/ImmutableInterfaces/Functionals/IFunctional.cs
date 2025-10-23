using SciDevOOP.ImmutableInterfaces.Functions;

namespace SciDevOOP.ImmutableInterfaces.Functionals;

interface IFunctional
{
    /// <summary>
    /// Method, that calculates functional value.
    /// </summary>
    /// <param name="function">Function that'll be calculated.</param>
    /// <returns>Functional value.</returns>
    double Value(IFunction function);
}