using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.GradientableAlgorithmTools.LimitingMethods;

public class PenaltyMethod : ILimitingMethod
{
    private double _alpha = 1.0D;
    private double _beta = 1.0D;

    void ILimitingMethod.Limit(ref double functionalValue, IVector parameters, IVector? minimalParameters, IVector? maximalParameters)
    {
        if (minimalParameters is not null)
        {
            for (var i = 0; i < minimalParameters.Count; ++i)
                functionalValue += _alpha * Math.Pow(Math.Max(0, minimalParameters[i] - parameters[i]), 2);
            //_alpha *= 2.0D;
        }
        if (maximalParameters is not null)
        {
            for (var i = 0; i < maximalParameters.Count; ++i)
                functionalValue += _beta * Math.Pow(Math.Max(0, parameters[i] - maximalParameters[i]), 2);
            //_beta *= 2.0D;
        }
    }
}