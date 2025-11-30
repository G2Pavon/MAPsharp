namespace MAPsharp.Lib.Converters.vmf2map;
public class Vmf2MapOptions : IConverterOptions
{
    public bool FlagConvertEntities { get; set; } = true;     
    public bool FlagRemoveEntities { get; set; } = true;       
    public bool FlagReplaceMaterials { get; set; } = true;
    public HashSet<string>? RemoveKey { get; set; }
    public HashSet<string>? RemoveEntity { get; set; }
    public List<PropertyRule>? Replace { get; set; }
    public Dictionary<string, string>? ReplaceMaterials { get; set; }

}
public class PropertyRule
{
    public string OldKey { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string NewKey { get; set; } = string.Empty;
    public string? NewValue { get; set; }
}