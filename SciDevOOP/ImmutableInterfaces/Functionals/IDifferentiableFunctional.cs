using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functionals;

interface IDifferentiableFunctional : IFunctional
{
    /// <summary>
    /// Method, that finds functional's gradient.
    /// </summary>
    /// <param name="function">Function, that should be derivated.</param>
    /// <returns>Vector of derivatives.</returns>
    IVector Gradient(IFunction function);
}