using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

interface INormable : IVector
{
    /// <summary>
    /// Method, that finds vector's norm.
    /// </summary>
    /// <returns>Vector's norm.</returns>
    public double Norma();
}