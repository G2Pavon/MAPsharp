using System.Diagnostics;

using MAPsharp.Lib;
using MAPsharp.Lib.Converters;
using MAPsharp.Lib.Formats.map;

namespace MAPsharp.CLI;

class Program
{
    private const string AppVersion = "1.0.0";

    static void Main(string[] args)
    {
        Logger.Header(AppVersion);

        if (ShouldShowHelp(args)) return;

        try
        {
            string inputPath = args[0];
            string outputPath = args.Length >= 2 ? args[1] : Path.ChangeExtension(inputPath, ".map");
            string configPath = Path.Combine(AppContext.BaseDirectory, "MAPsharp.json");

            Logger.Step("Validating file");
            Logger.Info($"Found: {inputPath}");
            if (!File.Exists(inputPath)) throw new FileNotFoundException($"Input file not found: {inputPath}");
            
            Logger.Step("Selecting converter strategy");
            IMapConverter converter = ConverterFactory.GetConverterForFile(inputPath);
            Logger.Info($"Detected format: {Path.GetExtension(inputPath)}");
            Logger.Info($"Using strategy:  {converter.GetType().Name}");

            Logger.Step("Loading strategy configuration");
            IConverterOptions options = converter.LoadOptions(configPath);

            Logger.Step("Converting");
            var stopwatch = Stopwatch.StartNew();

            Map mapOutput;
            try 
            {
                mapOutput = converter.Convert(inputPath, options);
                Logger.Info($"Conversion completed");
            }
            catch (Exception conversionEx)
            {
                Logger.Error("Conversion failed during processing.", conversionEx);
                return;
            }

            Logger.Step($"Saving file");
            try
            {
                mapOutput.Save(outputPath);
                Logger.Info($"Saved file: {Path.GetFullPath(outputPath)}");
            }
            catch (Exception saveEx)
            {
                Logger.Error($"Failed to write: {Path.GetFullPath(outputPath)}", saveEx);
                return;
            }

            stopwatch.Stop();
            Logger.Info($"\nTime elapsed: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
            Logger.Footer(outputPath);
        }
        catch (Exception ex)
        {
            Logger.Error("Unexpected error.", ex);
        }
    }
    
    private static bool ShouldShowHelp(string[] args)
    {
        if (args.Length < 1 || args[0] == "--help" || args[0] == "-h")
        {
            Logger.Msg("Usage: MAPsharp <input.vmf> \nOptional: MAPsharp <input.vmf> [output.map]");
            Logger.Msg("Convert VMF to MAP");
            Logger.Msg("https://github.com/G2Pavon/MAPsharp");
            return true;
        }
        if (args[0] == "--version" || args[0] == "-v") return true;
        return false;
    }
}