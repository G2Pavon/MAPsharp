namespace MAPsharp.Formats.vmf
{

    public class VmfEntity
    {
        public int Id { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new();
        public List<VmfSolid> Solids { get; set; } = new();
    }
}