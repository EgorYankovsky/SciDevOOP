using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.GradientableAlgorithmTools;

interface ILimitingMethod
{
    /// <summary>
    /// Method, that recalculates functional's value if parameters out of minimal or maximal parameters.
    /// </summary>
    /// <param name="functionalValue">Functional's value</param>
    /// <param name="parameters">Current parameters.</param>
    /// <param name="minimalParameters">Minimal parameters</param>
    /// <param name="maximalParameters">Maximal parameters</param>
    void Limit(ref double functionalValue, IVector parameters, IVector? minimalParameters, IVector? maximalParameters);
}