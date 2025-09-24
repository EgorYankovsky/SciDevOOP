using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

public class Polynomial : IFunction
{
    double IFunction.Value(IVector point)
    {
        throw new NotImplementedException();
    }
}