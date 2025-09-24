using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals;

public class L1Norm : IDifferentiableFunctional
{
    IVector IDifferentiableFunctional.Gradient(IFunction function)
    {
        throw new NotImplementedException();
    }

    double IFunctional.Value(IFunction function)
    {
        throw new NotImplementedException();
    }
}