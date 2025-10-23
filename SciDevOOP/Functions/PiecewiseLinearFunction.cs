using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functions;

class PiecewiseLinearFunction : IParametricFunction
{
    /// <summary>
    /// Represents a piecewise linear function of the form:
    ///         /
    ///         | f0(x) x < x0
    ///         | f1(x) x0 <= x < x1
    /// f(x) = <  f2(x) x1 <= x < x2
    ///         | ...
    ///         | fn(x) x >= xn_1
    ///         \
    /// where
    /// fi(x) => ai * x + bi * y = c
    /// </summary>
    class InternalPiecewiseLinearFunction : IDifferentiableFunction
    {
        private readonly double _h = 1e-8;

        public IVector? x; // x0, x1, ... xn_1.
        public IVector? a; // a0, a1, ... an;
        public IVector? b; // b0, b1, ... bn;
        public IVector? c; // b0, b1, ... bn;

        /// <summary>
        /// Method, that finds index of linear function according to point.
        /// </summary>
        /// <param name="point">Point to search.</param>
        /// <returns>Index of linear function.</returns>
        private int FindPiecewiseIndex(IVector point)
        {
            var indexLeft = 0;
            var indexRight = x!.Count - 1;
            while (indexLeft <= indexRight)
            {
                var middleIndex = (indexLeft + indexRight) / 2;
                if (point[0] == x![middleIndex])
                {
                    indexLeft = middleIndex;
                    break;
                }
                else if (point[0] < x[middleIndex])
                    indexRight = middleIndex - 1;
                else if (point[0] > x[middleIndex])
                    indexLeft = middleIndex + 1;
            }
            return indexLeft;
        }

        IVector IDifferentiableFunction.Gradient(IVector point)
        {
            var gradient = new Vector();
            var baseValue = (this as IFunction)!.Value(point);
            for (var i = 0; i < x!.Count + a!.Count + b!.Count + c!.Count; ++i)
            {
                var coefficientsCopy = new Vector(x);
                coefficientsCopy.AddRange(a);
                coefficientsCopy.AddRange(b);
                coefficientsCopy.AddRange(c);
                // If parameter is too little, we shall find derivative with other way.
                coefficientsCopy[i] += _h;
                var f1 = new PiecewiseLinearFunction().Bind(coefficientsCopy);
                var derivative = (f1.Value(point) - baseValue) / _h;
                gradient.Add(derivative);
            }
            return gradient;
        }

        /// <summary>
        /// Functions, that calculates f(x). Binary search must be successfully tested!
        /// </summary>
        /// <param name="point">Point where calculation is necessary.</param>
        /// <returns>Calculated value.</returns>
        double IFunction.Value(IVector point)
        {
            if (x is null) throw new Exception("Vector x is null at InternalPiecewiseLinearFunction.Value.");
            if (a is null) throw new Exception("Vector a is null at InternalPiecewiseLinearFunction.Value.");
            if (b is null) throw new Exception("Vector b is null at InternalPiecewiseLinearFunction.Value.");
            if (c is null) throw new Exception("Vector c is null at InternalPiecewiseLinearFunction.Value.");
            var index = FindPiecewiseIndex(point);
            return -1.0 / b[index] * (a[index] * point[0] - c[index]);
        }
    }

    /// <summary>
    /// Method, that checks correctness of input parameters.
    /// </summary>
    /// <param name="parameters">Input parameters.</param>
    /// <param name="n">Parameters amount.</param>
    /// <exception cref="ArgumentException">Raises if input data contains mistakes.</exception>
    private void CheckParameters(IVector parameters, int n)
    {
        for (var i = 0; i < n; ++i)
        {
            var zeros = 0;
            if (parameters[n - 1 + i] == 0) zeros++;
            if (parameters[2 * n - 1 + i] == 0) zeros++;
            if (parameters[3 * n - 1 + i] == 0) zeros++;
            if (zeros > 1)
            {
                var leftX = i == 0 ? "-inf" : parameters[i - 1].ToString();
                var rightX = i == n - 1 ? "inf" : parameters[i].ToString();
                var a = parameters[n - 1 + i];
                var b = parameters[2 * n - 1 + i];
                var c = parameters[3 * n - 1 + i];
                throw new ArgumentException($"Incorrect input data for x in ({leftX}; {rightX}): a = {a}, b = {b}, c = {c}.");
            }
        }

    }

    /// <summary>
    /// Method, that binds parameters to function. Must be successfully tested!
    /// </summary>
    /// <param name="parameters">parameters = [x0, x1, ... xn_1, a0, a1, ... an, b0, b1, ... bn, c0, c1, ... cn]</param>
    /// <returns>Generated InternalPiecewiseLinearFunction class.</returns>
    public IFunction Bind(IVector parameters)
    {
        var n = (parameters.Count + 1) / 4;
        CheckParameters(parameters, n);
        return new InternalPiecewiseLinearFunction()
        {
            x = new Vector(parameters.Take(n - 1)),
            a = new Vector(parameters.Skip(n - 1).Take(n)),
            b = new Vector(parameters.Skip(2 * n - 1).Take(n)),
            c = new Vector(parameters.Skip(3 * n - 1).Take(n))
        };
    }
}