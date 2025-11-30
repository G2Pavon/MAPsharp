namespace MAPsharp.Formats.map
{
    public class MapEntity
    {
        public Dictionary<string, string> Properties { get; set; } = new();
        public List<MapBrush> Brushes { get; set; } = new();
    }
}