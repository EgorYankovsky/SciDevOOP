using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functionals;

interface ILeastSquaresFunctional : IFunctional
{
    IVector Residual(IFunction function);
    IMatrix Jacobian(IFunction function);
}