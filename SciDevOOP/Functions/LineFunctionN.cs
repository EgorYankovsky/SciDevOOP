using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

public class LineFunctionN : IParametricFunction
{
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

    IFunction IParametricFunction.Bind(IVector parameters)
        => new InternalLineFunctionN() { coefficients = parameters };
}