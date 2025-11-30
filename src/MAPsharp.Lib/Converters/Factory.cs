using MAPsharp.Lib.Converters.vmf2map;

namespace MAPsharp.Lib.Converters;

public static class ConverterFactory
{
    private static readonly List<IMapConverter> Converters = new()
    {
        new VmfToMap(),
    };

    public static IMapConverter GetConverterForFile(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        var converter = Converters.FirstOrDefault(c => c.CanConvert(extension));

        if (converter == null)
        {
            throw new NotSupportedException($"No converter found for extension: {extension}");
        }

        return converter;
    }
}