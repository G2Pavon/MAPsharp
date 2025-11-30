using System.Globalization;

namespace MAPsharp.Formats.vmf
{

    public class Vmf
    {
        public string FilePath = "";
        public List<VmfEntity> Entities { get; set; } = new();

        public Vmf()
        {
        }

        public Vmf(string filePath)
        {
            FilePath = filePath;
            var doc = new Document(filePath);

            foreach (var node in doc.RootNodes)
            {
                switch (node.Class)
                {
                    case "world":
                        Entities.Add(ParseEntity(node));
                        break;
                    case "entity":
                        Entities.Add(ParseEntity(node));
                        break;
                    // TODO: Parse all nodes
                    default:
                        break;
                }
            }
        }

        private static VmfEntity ParseEntity(VmfNode vmfNode)
        {
            var entity = new VmfEntity();

            foreach (var kv in vmfNode.Properties)
                entity.Properties[kv.Key] = kv.Value;

            foreach (var child in vmfNode.Children)
            {
                if (child.Class == "solid")
                    entity.Solids.Add(ParseSolid(child));

            }

            return entity;
        }

        private static VmfSolid ParseSolid(VmfNode vmfNode)
        {
            var solid = new VmfSolid();

            foreach (var child in vmfNode.Children)
            {
                if (child.Class == "side")
                    solid.Sides.Add(ParseSide(child));
            }

            return solid;
        }

        private static VmfSide ParseSide(VmfNode vmfNode)
        {
            var side = new VmfSide();

            foreach (var kv in vmfNode.Properties)
            {
                switch (kv.Key)
                {
                    case "id":
                        side.Id = int.Parse(kv.Value);
                        break;
                    case "plane":
                        side.Plane = ParsePlane(kv.Value);
                        break;
                    case "material":
                        side.Material = kv.Value;
                        break;
                    case "uaxis":
                        side.Uaxis = ParseAxis(kv.Value);
                        break;
                    case "vaxis":
                        side.Vaxis = ParseAxis(kv.Value);
                        break;
                    case "rotation":
                        side.Rotation = float.Parse(kv.Value, CultureInfo.InvariantCulture);
                        break;
                    case "lightmapscale":
                        side.LightmapScale = int.Parse(kv.Value);
                        break;
                    case "smoothing_groups":
                        side.SmoothingGroups = int.Parse(kv.Value);
                        break;
                    case "contents":
                        side.Contents = int.Parse(kv.Value);
                        break;
                    case "flags":
                        side.Flags = int.Parse(kv.Value);
                        break;
                }
            }

            return side;
        }

        private static (double X, double Y, double Z)[] ParsePlane(string text)
        {
            // "(x y z) (x y z) (x y z)"
            var parts = text.Replace("(", "").Replace(")", "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new (double, double, double)[3];
            for (int i = 0; i < 3; i++)
            {
                result[i] = (
                    double.Parse(parts[i * 3], CultureInfo.InvariantCulture),
                    double.Parse(parts[i * 3 + 1], CultureInfo.InvariantCulture),
                    double.Parse(parts[i * 3 + 2],CultureInfo.InvariantCulture)
                );
            }

            return result;
        }

        private static float[] ParseAxis(string text)
        {
            // "[x y z offset] scale"
            text = text.Replace("[", "").Replace("]", "");
            var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new float[5];
            for (int i = 0; i <= 4; i++)
                result[i] = float.Parse(parts[i], CultureInfo.InvariantCulture);
            return result;
        }
    }
}