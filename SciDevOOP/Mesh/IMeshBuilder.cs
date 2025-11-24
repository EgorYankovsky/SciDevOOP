using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Mesh;

interface IMeshBuilder
{
    IMesh Build(IList<IVector> uniques);
}