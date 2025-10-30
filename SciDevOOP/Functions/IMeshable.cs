using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functions;

interface IMeshable
{
    /// <summary>
    /// Method, that returns coordinates of mesh above x-axis in some functions.
    /// </summary>
    /// <returns>Vector of mesh [x0, x1, ..., xn].</returns>
    IVector GetMesh();
}