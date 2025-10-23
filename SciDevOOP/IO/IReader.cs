namespace SciDevOOP.IO;

interface IReader
{
    /// <summary>
    /// Method, that reads information.
    /// </summary>
    /// <returns>Vector of initial points.</returns>
    IList<IList<double>>? Read();
}