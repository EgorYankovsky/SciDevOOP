using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;
using System.Drawing;

namespace SciDevOOP.Functionals;

class SimpleIntegrationNorm : IFunctional
{
    public IList<IList<double>> points;

    double IFunctional.Value(IFunction function)
    {
        if (points == null || points.Count == 0) return 0;

        double sumSquaredError = 0;
        int count = 0;

        foreach (var dataPoint in points)
        {
            if (dataPoint.Count < 2) continue;

            // Берем только координаты (без значения)
            var coordinates = new Vector(dataPoint.Take(dataPoint.Count - 1).ToList());
            double functionValue = function.Value(coordinates);
            double dataValue = dataPoint[dataPoint.Count - 1];

            double error = functionValue - dataValue;
            sumSquaredError += error * error;
            count++;
        }

        return count > 0 ? Math.Sqrt(sumSquaredError / count) : double.MaxValue;
    }
}