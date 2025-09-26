using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

class PiecewiseLinearFunction : IParametricFunction
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
        public IVector? x;
        public IVector? y;

        //public IVector? delimiters; // x0, x1, ... xn_1.
        //public IVector? a; // a0, a1, ... an;
        //public IVector? b; // b0, b1, ... bn;

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
            var x0 = x![indexLeft];
            var y0 = y![indexLeft];
            
            var x1 = x![indexLeft + 1];
            var y1 = y![indexLeft + 1];

            var a = (y1 - y0) / (x1 - x0);
            var b = y0 - a * x0;

            return a * point[0] + b;
        }
    }

    /// <summary>
    /// Method, that binds parameters to function. Must be successfully tested!
    /// </summary>
    /// <param name="parameters">parameters = [x0, x1, ... xn, y0, y1, ... yn]</param>
    /// <returns>Generated InternalPiecewiseLinearFunction class.</returns>
    public IFunction Bind(IVector parameters)
    {
        var n = parameters.Count / 2;

        // Better to improve with LINQ.
        var _x = new Vector();
        for (var i = 0; i < n; ++i) _x.Add(parameters[i]);

        var _y = new Vector();
        for (var i = n; i < 2 * n; ++i) _y.Add(parameters[i]);


        return new InternalPiecewiseLinearFunction()
        {
            x = _x,
            y = _y,
        };
    }
}