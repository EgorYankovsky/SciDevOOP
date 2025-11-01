using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools;

interface IMesh
{
    (IList<int>?, IList<IVector>?) FindElem(IVector point);
    void Bind(IVector parameters);
}