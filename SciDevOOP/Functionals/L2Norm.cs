using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals;

public class L2Norm : IDifferentiableFunctional, ILeastSquaresFunctional
{
    IVector IDifferentiableFunctional.Gradient(IFunction function)
    {
        throw new NotImplementedException();
    }

    IMatrix ILeastSquaresFunctional.Jacobian(IFunction function)
    {
        throw new NotImplementedException();
    }

    IVector ILeastSquaresFunctional.Residual(IFunction function)
    {
        throw new NotImplementedException();
    }

    double IFunctional.Value(IFunction function)
    {
        throw new NotImplementedException();
    }
}