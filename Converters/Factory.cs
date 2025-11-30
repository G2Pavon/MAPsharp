using MAPsharp.Core;

namespace MAPsharp.Converters;

public static class ConverterFactory
{
    private static readonly List<IMapConverter> _converters = new()
    {
        new VmfConverter(),
    };

    public static IMapConverter GetConverterForFile(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        var converter = _converters.FirstOrDefault(c => c.CanConvert(extension));

        if (converter == null)
        {
            throw new NotSupportedException($"No converter found for extension: {extension}");
        }

        return converter;
    }
}