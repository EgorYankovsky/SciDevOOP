using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

public class PiecewiseLinearFunction : IDifferentiableFunction
{
    IVector IDifferentiableFunction.Gradient(IVector point)
    {
        throw new NotImplementedException();
    }

    double IFunction.Value(IVector point)
    {
        throw new NotImplementedException();
    }
}