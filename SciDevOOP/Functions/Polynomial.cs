using System.Text;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

class Polynomial : IParametricFunction
{
    /// <summary>
    /// Represents a polynomial function of the form:
    /// P(x) = c_0 + c_1 * x + c_2 * x^2 + ... + c_n * x^n.
    /// </summary>
    class InternalPolynomial : IFunction, IWritableFunction
    {
        public IVector? coefficients;

        string IWritableFunction.ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < (coefficients?.Count ?? 0); ++i)
                sb.AppendLine($"c_{i} = {coefficients![i]}");
            return sb.ToString();
        }

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

    public IFunction Bind(IVector parameters)
        => new InternalPolynomial() { coefficients = parameters };
}