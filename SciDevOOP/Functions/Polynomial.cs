using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

public class Polynomial : IParametricFunction
{
    /// <summary>
    /// Represents a polynomial function of the form:
    /// P(x) = c_0 + c_1 * x + c_2 * x^2 + ... + c_n * x^n.
    /// </summary>
    class InternalPolynomial : IFunction
    {
        public IVector? coefficients;

        double IFunction.Value(IVector point)
        {
            var sum = 0.0;
            var value = 1.0;
            foreach (var c in coefficients!)
            {
                sum += c * value;
                value *= point[0];
            }
            return sum;
        }
    }

    IFunction IParametricFunction.Bind(IVector parameters)
        => new InternalPolynomial() { coefficients = parameters };

}