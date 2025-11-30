using MAPsharp.Lib.Formats.map;

namespace MAPsharp.Lib;

public interface IMapConverter
{
    bool CanConvert(string extension);
    IConverterOptions LoadOptions(string? configPath);
    Map Convert(string inputPath, IConverterOptions options);
}