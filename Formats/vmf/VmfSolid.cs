namespace MAPsharp.Formats.vmf
{

    public class VmfSolid
    {
        public int Id { get; set; }
        public List<VmfSide> Sides { get; set; } = new();
    }
}