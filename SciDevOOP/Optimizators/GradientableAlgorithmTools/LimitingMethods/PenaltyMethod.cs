using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.GradientableAlgorithmTools.LimitingMethods;

public class PenaltyMethod : ILimitingMethod
{
    private double _alpha = 2.0D;
    private double _beta = 2.0D;

    void ILimitingMethod.Limit(ref double functionalValue, IVector parameters, IVector? minimalParameters, IVector? maximalParameters)
    {
        if (minimalParameters is not null)
        {
            var minimalPenaltySum = 0.0D;
            for (var i = 0; i < minimalParameters.Count; ++i)
                minimalPenaltySum += _alpha * Math.Pow(Math.Max(0, minimalParameters[i] - parameters[i]), 2);
            if (minimalPenaltySum > 0.0D) _alpha *= 1.5D;
            functionalValue += minimalPenaltySum;
        }
        if (maximalParameters is not null)
        {
            var maximalPenaltySum = 0.0D;
            for (var i = 0; i < maximalParameters.Count; ++i)
                maximalPenaltySum += _beta * Math.Pow(Math.Max(0, parameters[i] - maximalParameters[i]), 2);
            if (maximalPenaltySum > 0.0D) _beta *= 1.5D;
            functionalValue += maximalPenaltySum;
        }
    }
}