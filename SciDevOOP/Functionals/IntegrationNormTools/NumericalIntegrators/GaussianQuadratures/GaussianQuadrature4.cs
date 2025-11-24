using System.Collections.Immutable;
using SciDevOOP.Functionals.IntegrationNormTools.NumericalIntegrators;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools.NumericalIntegrators.GaussianQuadratures;

class GaussianQuadrature4 : GaussianQuadrature
{
    private ImmutableArray<double> w = [0.3478548451374538,
                                        0.6521451548625461,
                                        0.6521451548625461,
                                        0.3478548451374538];

    private ImmutableArray<double> t = [-0.8611363115940526,
                                        -0.3399810435848563,
                                         0.3399810435848563,
                                         0.8611363115940526];


    public override double Integrate(IFunction function, IEnumerable<(double, double)> limits)
    {
        var indexSwitcher = CreateIndexSwitcher(limits.Count(), 4);
        var ans = 0.0D;

        for (int i = 0; i < Math.Pow(4, limits.Count()); ++i)
        {
            var indx = indexSwitcher.GetIndexes();
            var x = new Vector();
            for (int ii = 0; ii < limits.Count(); ++ii)
            {
                x.Add(0.5 * (limits.ElementAt(ii).Item2 - limits.ElementAt(ii).Item1) * t[indx[ii]] 
                    + 0.5 * (limits.ElementAt(ii).Item1 + limits.ElementAt(ii).Item2));
            }
            var W = 1.0D;
            foreach (var ind in indx) W *= w[ind];
            var f = function.Value(x);
            ans += W * f;
            indexSwitcher.UpdateIndex();
        }
        foreach (var limit in limits) ans *= 0.5 * (limit.Item2 - limit.Item1);
        return ans;
    }
}