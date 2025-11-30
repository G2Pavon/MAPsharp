namespace MAPsharp.Lib.Formats.map
{
    public class MapFace
    {
        public (double X, double Y, double Z)[] Plane { get; set; } = new (double, double, double)[3];
        public string Texture { get; set; } = "";
        public float[] Uaxis { get; set; } = new float[4];
        public float[] Vaxis { get; set; } = new float[4];
        public float Rotation { get; set; } = 0;
        public float Uscale { get; set; } = 1;
        public float Vscale { get; set; } = 1;

    }
}