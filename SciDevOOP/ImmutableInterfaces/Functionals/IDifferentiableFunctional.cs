using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functionals;

interface IDifferentiableFunctional : IFunctional
{
    IVector Gradient(IFunction function);
}