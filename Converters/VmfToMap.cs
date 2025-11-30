using System.Text.Json;
using MAPsharp.Core;
using MAPsharp.Formats.map;
using MAPsharp.Formats.vmf;

namespace MAPsharp.Converters;

public class VmfConverter : IMapConverter
{
    private readonly Dictionary<string, string> _materialCache = new(StringComparer.OrdinalIgnoreCase);
    public bool CanConvert(string extension)
    {
        return extension.Equals(".vmf", StringComparison.OrdinalIgnoreCase);
    }

    public IConverterOptions LoadOptions(string? configPath)
    {
        if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
        {
            Logger.Warning("VMF rules file not found");
            return new VmfOptions();
        }
        
        Logger.Info($"Loading rules: {configPath}");
        try 
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string jsonString = File.ReadAllText(configPath);
            var rules = JsonSerializer.Deserialize<VmfOptions>(jsonString, jsonOptions) ?? new VmfOptions();

            if (rules.RemoveKey != null)
                rules.RemoveKey = new HashSet<string>(rules.RemoveKey.Select(r => r.ToLower()));
            if (rules.RemoveEntity != null)
                rules.RemoveEntity = new HashSet<string>(rules.RemoveEntity.Select(r => r.ToLower()));
            Logger.Info($"Loaded rules: {configPath}");
            return rules;
        }
        catch (Exception ex)
        {
            Logger.Error($"Loading rules: {ex.Message}");
            return new VmfOptions();
        }
    }
    
    public Map Convert(string inputPath, IConverterOptions options)
    {
        var rules = options as VmfOptions ?? new VmfOptions();
        
        Logger.Info($"Loading file: {inputPath}");
        var input = new Vmf(inputPath);
        
        Map output = new();
        List<MapEntity> entities = new();
        int statsIgnored = 0;
        int statsProcessed = 0;

        foreach (var ent in input.Entities)
        {
            if ( rules.FlagRemoveEntities && rules.RemoveEntity != null &&
                 ent.Properties.TryGetValue("classname", out string? classname) &&
                 rules.RemoveEntity.Contains(classname.ToLower()))
            {
                statsIgnored++;
                continue;
            }

            var convertedEntity = ConvertEntity(ent, rules);
            if (convertedEntity != null)
            {
                entities.Add(convertedEntity);
                statsProcessed++;
            }
        }
        output.Entities = entities;

        Logger.Info("Entities:");
        Logger.Info($"   Converted: {statsProcessed}");
        Logger.Info($"   Removed:   {statsIgnored}");
        return output;
    }

    private MapEntity? ConvertEntity(VmfEntity sourceVmfEntity, VmfOptions rules)
    {
        var propsProcessed = new Dictionary<string, string>(sourceVmfEntity.Properties);
        if (rules.FlagConvertEntities)
        {
            if (rules.Replace != null)
            {
                var updates = new List<(string keyToRemove, string keyToAdd, string valueToAdd)>();
                foreach (var rule in rules.Replace)
                {
                    var match = propsProcessed.FirstOrDefault(p => p.Key.ToLower() == rule.OldKey);
                    if (!string.IsNullOrEmpty(match.Key))
                    {
                        if (rule.OldValue == null || match.Value == rule.OldValue)
                        {
                            updates.Add((match.Key, rule.NewKey, rule.NewValue ?? match.Value));
                        }
                    }
                }

                foreach (var upd in updates)
                {
                    propsProcessed.Remove(upd.keyToRemove);
                    propsProcessed[upd.keyToAdd] = upd.valueToAdd;
                }
            }


            if (rules.RemoveKey != null)
            {
                foreach (var prop in propsProcessed.Keys.ToList())
                {
                    if (rules.RemoveKey.Contains(prop.ToLower())) propsProcessed.Remove(prop);
                }
            }
        }

        if (!propsProcessed.ContainsKey("classname") && sourceVmfEntity.Properties.ContainsKey("classname"))
        {
            propsProcessed["classname"] = sourceVmfEntity.Properties["classname"];
        }

        var goldsrcEntity = new MapEntity
        {
            Properties = propsProcessed,
            Brushes = sourceVmfEntity.Solids.Select(s => ConvertToBrush(s, rules)).ToList()
        };

        return goldsrcEntity;
    }

    private MapBrush ConvertToBrush(VmfSolid sourceVmfSolid, VmfOptions rules)
    {
        return new MapBrush
        {
            Faces = sourceVmfSolid.Sides.Select(s => ConvertSide(s, rules)).ToList()
        };
    }

    private MapFace ConvertSide(VmfSide sourceVmfSide, VmfOptions rules)
    {
        return new MapFace
        {
            Plane = sourceVmfSide.Plane,
            Texture = ConvertMaterial(sourceVmfSide.Material, rules),
            Uaxis = new[] { sourceVmfSide.Uaxis[0], sourceVmfSide.Uaxis[1], sourceVmfSide.Uaxis[2], sourceVmfSide.Uaxis[3] },
            Uscale = sourceVmfSide.Uaxis[4],
            Vaxis = new[] { sourceVmfSide.Vaxis[0], sourceVmfSide.Vaxis[1], sourceVmfSide.Vaxis[2], sourceVmfSide.Vaxis[3] },
            Vscale = sourceVmfSide.Vaxis[4],
            Rotation = sourceVmfSide.Rotation
        };
    }
    private string ConvertMaterial(string sourceMaterial, VmfOptions rules)
    {
        if (_materialCache.TryGetValue(sourceMaterial, out string? cachedMaterial)) return cachedMaterial;

        string textureName = Path.GetFileName(sourceMaterial).ToLower().Replace(" ", "_");
        
        if (rules.FlagReplaceMaterials && rules.ReplaceMaterials != null && rules.ReplaceMaterials.TryGetValue(textureName, out string? replacedTextureName))
        {
            textureName = replacedTextureName;
        }
        // TODO: limit length names but at the same time avoid repeated names
        // if (textureName.Length > 15) textureName = textureName.Substring(textureName.Length - 15);
        
        _materialCache[sourceMaterial] = textureName;
        return textureName;
    }
}