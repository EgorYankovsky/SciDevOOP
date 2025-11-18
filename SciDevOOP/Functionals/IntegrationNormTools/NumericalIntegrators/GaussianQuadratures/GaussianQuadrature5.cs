using SciDevOOP.Functionals.IntegrationNormTools.NumericalIntegrators;
using SciDevOOP.ImmutableInterfaces.Functions;
namespace SciDevOOP.Functionals.IntegrationNormTools.NumericalIntegrators.GaussianQuadratures;

class GaussianQuadrature5 : GaussianQuadrature
{
    public override double Integrate(IFunction function, IEnumerable<(double, double)> limits)
    {
        throw new NotImplementedException();
    }
}