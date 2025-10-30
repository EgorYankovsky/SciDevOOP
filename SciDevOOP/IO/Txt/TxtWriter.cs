using SciDevOOP.Functions;

namespace SciDevOOP.IO.Txt;

public class TxtWriter : IWriter
{
#if DEBUG
    private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory.Replace("Debug\\net9.0\\", "Resources\\");
#endif

#if RELEASE
    private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory.Replace("Release\\net9.0\\", "Resources\\");
#endif

    private readonly FileInfo? _file;

    public TxtWriter(string path)
    {
        _file = path.Split('\\').Length == 1 ? new FileInfo(_basePath + path) : new FileInfo(path);
    }

    public void Write(IList<double> values)
    {
        if (!_file!.Exists) throw new FileNotFoundException(_file.FullName);
        if (_file.IsReadOnly) throw new NotSupportedException($"Writing at {_file.FullName} is not supported.");
        using var sw = new StreamWriter(_file.FullName);
        foreach (var value in values)
            sw.WriteLine(value);
    }

    public void Write(IWritableFunction function)
    {
        if (!_file!.Exists) throw new FileNotFoundException(_file.FullName);
        if (_file.IsReadOnly) throw new NotSupportedException($"Writing at {_file.FullName} is not supported.");
        using var sw = new StreamWriter(_file.FullName);
        sw.WriteLine(function.ToString());
    }
}