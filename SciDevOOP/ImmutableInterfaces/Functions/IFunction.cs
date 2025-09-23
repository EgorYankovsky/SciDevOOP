using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functions;

interface IFunction
{
    double Value(IVector point);
}