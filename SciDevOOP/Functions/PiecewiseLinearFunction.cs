using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

public class PiecewiseLinearFunction : IParametricFunction
{

    //         /
    //         | f0(x) x < x0
    //         | f1(x) x0 <= x < x1
    // f(x) = <  f2(x) x1 <= x < x2
    //         | ...
    //         | fn(x) x >= xn_1
    //         \
    //
    // fi(x) = ai * x + bi
    class InternalPiecewiseLinearFunction : IDifferentiableFunction
    {
        public IVector? delimiters; // x0, x1, ... xn_1.
        public IVector? a; // a0, a1, ... an;
        public IVector? b; // b0, b1, ... bn;

        IVector IDifferentiableFunction.Gradient(IVector point)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Functions, that calculates f(x). Binary search must be successfully tested!
        /// </summary>
        /// <param name="point">Point where calculation is necessary.</param>
        /// <returns>Calculated value.</returns>
        double IFunction.Value(IVector point)
        {
            var indexLeft = 0;
            var indexRight = a!.Count - 1;
            while (indexLeft <= indexRight)
            {
                var middleIndex = (indexLeft + indexRight) / 2;
                if (point[0] == delimiters![middleIndex])
                {
                    indexLeft = middleIndex;
                    break;
                }
                else if (point[0] < delimiters[middleIndex])
                    indexRight = middleIndex - 1;
                else if (point[0] > delimiters[middleIndex])
                    indexLeft = middleIndex + 1;
            }
            return a[indexLeft] * point[0] + b![indexLeft];
        }
    }

    /// <summary>
    /// Method, that binds parameters to function. Must be successfully tested!
    /// </summary>
    /// <param name="parameters">parameters = [x0, x1, ... xn_1, a0, a1, ... an, b0, b1, ... bn]</param>
    /// <returns>Generated InternalPiecewiseLinearFunction class.</returns>
    IFunction IParametricFunction.Bind(IVector parameters)
    {
        var n = (parameters.Count + 1) / 3;
        return new InternalPiecewiseLinearFunction()
        {
            delimiters = (IVector)parameters.Take(n - 1),
            a = (IVector)parameters.Skip(n - 1).Take(n),
            b = (IVector)parameters.Skip(2 * n - 1).Take(n),
        };
    }
}