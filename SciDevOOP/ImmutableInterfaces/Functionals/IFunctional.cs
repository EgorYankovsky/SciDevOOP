using SciDevOOP.ImmutableInterfaces.Functions;

namespace SciDevOOP.ImmutableInterfaces.Functionals;

interface IFunctional
{
    double Value(IFunction function);
}