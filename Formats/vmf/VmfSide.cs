namespace MAPsharp.Formats.vmf
{

    public class VmfSide
    {
        public int Id { get; set; }
        public (double X, double Y, double Z)[] Plane { get; set; } = new (double, double, double)[3];
        public string Material { get; set; } = "";
        public float[] Uaxis { get; set; } = new float[5];
        public float[] Vaxis { get; set; } = new float[5];
        public float Rotation { get; set; }
        public int LightmapScale { get; set; }
        public int SmoothingGroups { get; set; }
        public int Contents { get; set; }
        public int Flags { get; set; }
    }
}