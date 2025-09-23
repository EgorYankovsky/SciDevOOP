using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functions;

interface IDifferentiableFunction : IFunction
{
    // �� ���������� �������� IParametricFunction
    IVector Gradient(IVector point);
}