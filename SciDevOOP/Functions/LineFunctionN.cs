using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

public class LineFunctionN : IDifferentiableFunction
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