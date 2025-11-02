using System.Text;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functions;

class LineFunctionN : IParametricFunction
{
    /// <summary>
    /// Represents a line function at N dimension of the form:
    /// F = c0 + c1*x1 + c2*x2 + ... + cn*xn.
    /// </summary>
    class InternalLineFunctionN : IDifferentiableFunction, IWritableFunction
    {
        private readonly double _h = 1e-8;
        public IVector? coefficients;

        string IWritableFunction.ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < (coefficients?.Count ?? 0); ++i)
                sb.AppendLine($"c_{i} = {coefficients![i]}");
            return sb.ToString();
        }

        IVector IDifferentiableFunction.Gradient(IVector point)
        {
            var gradient = new Vector();
            var baseValue = (this as IFunction)!.Value(point);
            for (var i = 0; i < coefficients!.Count; ++i)
            {
                var coefficientsCopy = new Vector(coefficients);
                coefficientsCopy[i] += _h;
                var f1 = new LineFunctionN().Bind(coefficientsCopy);
                var derivative = (f1.Value(point) - baseValue) / _h;
                gradient.Add(derivative);
            }
            return gradient;
        }

        double IFunction.Value(IVector point)
        {
            if (point.Count != coefficients?.Count - 1)
                throw new ArgumentException("Points dimension isn't equal to coefficients.");
            var sum = coefficients![0];
            foreach (var (p, c) in point.Zip(coefficients.Skip(1)!))
                sum += p * c;
            return sum;
        }
    }

    public IFunction Bind(IVector parameters)
        => new InternalLineFunctionN() { coefficients = parameters };
}