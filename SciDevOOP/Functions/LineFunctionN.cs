using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functions;

class LineFunctionN : IParametricFunction
{
    /// <summary>
    /// Represents a line function at N dimension of the form:
    /// F = c0 + c1*x1 + c2*x2 + ... + cn*xn = 0.
    /// </summary>
    class InternalLineFunctionN : IDifferentiableFunction
    {
        private double _h = 1e-8;
        public IVector? coefficients;

        IVector IDifferentiableFunction.Gradient(IVector point)
        {
            var gradient = new Vector();
            for (var i = 0; i < coefficients!.Count; ++i)
            {
                var coefficientsCopy = new Vector(coefficients);
                // If parameter is too little, we shall find derivative with other way.
                coefficientsCopy[i] = coefficientsCopy[i] + _h;
                var f1 = new LineFunctionN().Bind(coefficientsCopy);
                var derivative = (f1.Value(point) - (this as IFunction).Value(point)) / _h;
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