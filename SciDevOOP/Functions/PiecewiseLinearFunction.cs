using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;
using System.Text;

namespace SciDevOOP.Functions;

class PiecewiseLinearFunction : IParametricFunction
{
    /// <summary>
    /// Represents a piecewise linear function of the form:
    /// f(x) = ax + b + c_0 * |x - x_0| + ... + c_n * |x - x_n|
    /// </summary>
    class InternalPiecewiseLinearFunction : IDifferentiableFunction, IMeshable, IWritableFunction
    {
        private readonly double _h = 1e-8;

        public IVector? xes; // x0, x1, ... xn_1.
        public IVector? c; // c0, c1, ... cn;
        public double a;
        public double b;

        string IWritableFunction.ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"a = {a}");
            sb.AppendLine($"b = {b}");
            for (int i = 0; i < (c?.Count ?? 0); ++i)
                sb.AppendLine($"c_{i} = {c![i]}");
            return sb.ToString();
        }

        IVector IMeshable.GetMesh() => xes is not null ? xes : new Vector();

        IVector IDifferentiableFunction.Gradient(IVector point)
        {
            var gradient = new Vector();
            var baseValue = (this as IFunction)!.Value(point);
            for (var i = 0; i < c!.Count + 2; ++i)
            {
                var coefficientsCopy = new Vector { a, b };
                coefficientsCopy.AddRange(c);
                coefficientsCopy.AddRange(xes!); // ?
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
    /// <param name="parameters">parameters = [a, b, c0, c1, ... cn, x0, x1, ... xn]</param>
    /// <returns>Generated InternalPiecewiseLinearFunction class.</returns>
    /// <exception cref="ArgumentException">Raises if input data was in incorrect format.</exception>
    /// <exception cref="Exception">Raises in case of truly bad input data.</exception>
    public IFunction Bind(IVector parameters)
    {
        if ((parameters.Count - 2) % 2 != 0) throw new ArgumentException("Incorrect input data.");
        var n = (parameters.Count - 2) / 2;
        return n switch
        {
            0 => new InternalPiecewiseLinearFunction()
            {
                a = parameters[0],
                b = parameters[1],
                xes = null,
                c = null
            },
            > 0 => new InternalPiecewiseLinearFunction()
            {
                a = parameters[0],
                b = parameters[1],
                c = new Vector(parameters.Skip(2).Take(n)),
                xes = new Vector(parameters.Skip(n + 2).Take(n))
            },
            _ => throw new Exception("Woah, raised exception, that shouln't be raised.")
        };
    }
}