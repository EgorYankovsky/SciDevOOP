using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.GradientableAlgorithmTools.LimitingMethods;

public class BarrierMethod : ILimitingMethod
{
    void ILimitingMethod.Limit(ref double functionalValue, IVector parameters, IVector? minimalParameters, IVector? maximalParameters)
    {
        throw new NotImplementedException();
    }
}