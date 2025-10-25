using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;
using System.Xml;

namespace SciDevOOP.Functions;

class PiecewiseLinearFunction : IParametricFunction
{
    /// <summary>
    /// Represents a piecewise linear function of the form:
    /// f(x) = ax + b + c_0 * |x - x_0| + ... + c_n * |x - x_n|
    /// </summary>
    class InternalPiecewiseLinearFunction : IDifferentiableFunction
    {
        private readonly double _h = 1e-8;

        public IVector? xes; // x0, x1, ... xn_1.
        public IVector? c; // b0, b1, ... bn;
        public double a;
        public double b;

        IVector IDifferentiableFunction.Gradient(IVector point)
        {
            var gradient = new Vector();
            var baseValue = (this as IFunction)!.Value(point);
            for (var i = 0; i < xes!.Count + c!.Count + 2; ++i)
            {
                var coefficientsCopy = new Vector(xes) { a, b };
                coefficientsCopy.AddRange(c);
                // If parameter is too little, we shall find derivative with other way.
                coefficientsCopy[i] += _h;
                var f1 = new PiecewiseLinearFunction().Bind(coefficientsCopy);
                var derivative = (f1.Value(point) - baseValue) / _h;
                gradient.Add(derivative);
            }
            return gradient;
        }

        double IFunction.Value(IVector point)
        {
            if (xes is null) throw new Exception("Vector x is null at InternalPiecewiseLinearFunction.Value.");
            if (c is null) throw new Exception("Vector c is null at InternalPiecewiseLinearFunction.Value.");
            var x = point[0];
            var value = b;
            value += a * x;
            for (var i = 0; i < xes.Count; ++i)
                value += c[i] * Math.Abs(x - xes[i]);
            return value;
        }
    }

    /// <summary>
    /// Method, that binds parameters to function. Must be successfully tested!
    /// </summary>
    /// <param name="parameters">parameters = [x0, x1, ... xn, a, b, c0, c1, ... cn]</param>
    /// <returns>Generated InternalPiecewiseLinearFunction class.</returns>
    /// <exception cref="ArgumentException">Raises if input data was in incorrect format.</exception>
    public IFunction Bind(IVector parameters)
    {
        if ((parameters.Count - 2) % 2 != 0) throw new ArgumentException("Incorrect input data.");
        var n = (parameters.Count - 2) / 2;
        return new InternalPiecewiseLinearFunction()
        {
            xes = new Vector(parameters.Take(n)),
            a = parameters[n],
            b = parameters[n + 1],
            c = new Vector(parameters.Skip(n + 2).Take(n))
        };
    }
}