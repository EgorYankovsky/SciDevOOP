using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

class LineFunctionN : IParametricFunction
{
    /// <summary>
    /// Represents a line function at N dimension of the form:
    /// F = c0 + c1*x1 + c2*x2 + ... + cn*xn = 0.
    /// </summary>
    class InternalLineFunctionN : IDifferentiableFunction
    {
        public IVector? coefficients;

        IVector IDifferentiableFunction.Gradient(IVector point)
        {
            throw new NotImplementedException();
        }
        
        double IFunction.Value(IVector point)
        {
            if (point.Count != coefficients?.Count)
                throw new ArgumentException("Points dimension isn't equal to coefficients.");
            double sum = 0;
            foreach (var (p, c) in point.Zip(coefficients!))
                sum += p * c;
            return sum;
        }
    }

    public IFunction Bind(IVector parameters)
        => new InternalLineFunctionN() { coefficients = parameters };
}