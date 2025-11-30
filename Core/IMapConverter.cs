using MAPsharp.Formats.map;

namespace MAPsharp.Core;

public interface IMapConverter
{
    bool CanConvert(string extension);
    IConverterOptions LoadOptions(string? configPath);
    Map Convert(string inputPath, IConverterOptions options);
}