using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.GradientableAlgorithmTools;

interface ILimitingMethod
{
    void Limit(ref double functionalValue, IVector parameters, IVector? minimalParameters, IVector? maximalParameters);
}