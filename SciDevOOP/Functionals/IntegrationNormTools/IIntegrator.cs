using SciDevOOP.ImmutableInterfaces.Functions;

namespace SciDevOOP.Functionals.IntegrationNormTools;

interface IIntegrator
{
    double Integrate(IFunction function, IEnumerable<(double, double)> limits);
}