using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.GradientableAlgorithmTools.LimitingMethods;

public class BarrierMethod : ILimitingMethod
{
    private double _alpha = 0.1D;
    private double _beta = 0.1D;

    void ILimitingMethod.Limit(ref double functionalValue, IVector parameters, IVector? minimalParameters, IVector? maximalParameters)
    {
        var penalty = 0.0D;
        for (var i = 0; i < parameters.Count; ++i)
        {
            if (minimalParameters is not null)
            {
                var diff = parameters[i] - minimalParameters[i];
                if (diff < 0.0D)
                {
                    functionalValue = double.PositiveInfinity;
                    return;
                }
                penalty -= Math.Log(diff);
            }
            if (maximalParameters is not null)
            {
                var diff = maximalParameters[i]- parameters[i];
                if (diff <= 0.0D)
                {
                    functionalValue = double.PositiveInfinity;
                    return;
                }
                penalty -= Math.Log(diff);
            }
        }
        functionalValue += _alpha * penalty;
    }
}