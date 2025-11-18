using SciDevOOP.Functionals.IntegrationNormTools;
using SciDevOOP.ImmutableInterfaces.Functions;

namespace SciDevOOP.Functionals.IntegrationNormTools.NumericalIntegrators;

abstract class GaussianQuadrature : IIntegrator
{
    
    protected class IndexSwitcher(int dimension, int quadratureStep)
    {
        private readonly int _quadratureStep = quadratureStep;

        private readonly int[] indexes = new int[dimension];

        public int[] GetIndexes() => indexes;

        public void UpdateIndex()
        {
            for (int i = indexes.Length - 1; i >= 0; --i)
            {
                if (indexes[i] + 1 < _quadratureStep)
                {
                    indexes[i] += 1;
                    break;       
                }
                else indexes[i] = 0;
            }
        }
    }

    protected static IndexSwitcher CreateIndexSwitcher(int dimension, int quadratureStep) 
        => new(dimension, quadratureStep);

    abstract public double Integrate(IFunction function, IEnumerable<(double, double)> limits);
}