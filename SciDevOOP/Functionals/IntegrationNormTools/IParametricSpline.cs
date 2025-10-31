using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools;

interface IParametricSpline
{
    ISpline Bind(IVector y, IVector mesh);
}