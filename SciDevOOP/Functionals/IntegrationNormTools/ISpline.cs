using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools;

interface ISpline
{
    double Value(IVector parameters);
}