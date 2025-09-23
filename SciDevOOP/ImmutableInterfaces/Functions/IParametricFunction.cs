using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functions;

interface IParametricFunction
{
    IFunction Bind(IVector parameters);
}
