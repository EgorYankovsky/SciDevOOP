using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functions;

interface IDifferentiableFunction : IFunction
{
    // По параметрам исходной IParametricFunction
    IVector Gradient(IVector point);
}