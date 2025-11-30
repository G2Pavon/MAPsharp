using System.Globalization;
using System.Text;

namespace MAPsharp.Lib.Formats.map
{

    public class Map
    {
        public string filePath = "";
        public List<MapEntity> Entities { get; set; } = new();
        public string Game { get; set; } = "Half-Life";
        private string Format = "Valve";

        public Map()
        {
            var worldspawn = new MapEntity();
            worldspawn.Properties.Add("classname", "worldspawn");
            worldspawn.Properties.Add("mapversion", "220");
            worldspawn.Properties.Add("wad","");
            this.Entities.Add(worldspawn);
        }
        public Map(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File {path} does not exist");
            }
            filePath = path;
            MapEntity? currentEntity = null;
            MapBrush? currentBrush = null;
            var state = ParseState.None;
            
            var lines = File.ReadAllLines(path);
            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    continue;
                }

                if (line == "{")
                {
                    if (state == ParseState.None)
                    {
                        currentEntity = new MapEntity();
                        state = ParseState.InEntity;
                    }
                    else if (state == ParseState.InEntity)
                    {
                        currentBrush = new MapBrush();
                        state = ParseState.InBrush;
                    }
                }
                else if (line == "}")
                {
                    if (state == ParseState.InBrush)
                    {
                        if (currentBrush != null && currentEntity != null)
                        {
                            currentEntity.Brushes.Add(currentBrush);
                        }

                        currentBrush = null;
                        state = ParseState.InEntity;
                    }
                    else if (state == ParseState.InEntity)
                    {
                        if (currentEntity != null)
                        {
                            Entities.Add(currentEntity);
                        }

                        currentEntity = null;
                        state = ParseState.None;
                    }
                }
                else
                {
                    if (state == ParseState.InEntity)
                    {
                        var parts = line.Split('"',
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        if (parts.Length >= 2 && currentEntity != null)
                        {
                            currentEntity.Properties[parts[0]] = parts[1];
                        }
                    }
                    else if (state == ParseState.InBrush)
                    {
                        if (line.StartsWith("("))
                        {
                            var face = new MapFace();
                            var parts = line.Split(' '); // lenght 31
                            face.Plane[0] = (float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                            face.Plane[1] = (float.Parse(parts[6]), float.Parse(parts[7]), float.Parse(parts[8]));
                            face.Plane[2] = (float.Parse(parts[11]), float.Parse(parts[12]), float.Parse(parts[13]));
                            face.Texture = parts[15];
                            face.Uaxis[0] = float.Parse(parts[17]);
                            face.Uaxis[1] = float.Parse(parts[18]);
                            face.Uaxis[2] = float.Parse(parts[19]);
                            face.Uaxis[3] = float.Parse(parts[20]);
                            face.Vaxis[0] = float.Parse(parts[23]);
                            face.Vaxis[1] = float.Parse(parts[24]);
                            face.Vaxis[2] = float.Parse(parts[25]);
                            face.Vaxis[3] = float.Parse(parts[26]);
                            face.Rotation = float.Parse(parts[28]);
                            face.Uscale = float.Parse(parts[29]);
                            face.Vscale = float.Parse(parts[30]);

                            if (currentBrush != null)
                            {
                                currentBrush.Faces.Add(face);
                            }
                        }
                    }
                }
            }

            if (currentEntity != null)
            {
                Entities.Add(currentEntity);
            }
        }

        public void Save(string path)
        {
            
            int entityNumber, brushNumber;
            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine($"// Game: {this.Game}\n// Format: {this.Format}");

                entityNumber = 0;
                foreach (var entity in this.Entities)
                {
                    writer.WriteLine($"// entity {entityNumber}");
                    writer.WriteLine("{");
                    foreach (var prop in entity.Properties)
                    {
                        writer.WriteLine($"\"{prop.Key}\" \"{prop.Value}\"");
                    }

                    brushNumber = 0;
                    foreach (var brush in entity.Brushes)
                    {
                        writer.WriteLine($"// brush {brushNumber}");
                        writer.WriteLine("{");
                        foreach (var face in brush.Faces)
                        {
                            var faceLine = new StringBuilder();

                            faceLine.Append(
                                $"( {face.Plane[0].X.ToString(CultureInfo.InvariantCulture)} {face.Plane[0].Y.ToString(CultureInfo.InvariantCulture)} {face.Plane[0].Z.ToString(CultureInfo.InvariantCulture)} ) ");
                            faceLine.Append(
                                $"( {face.Plane[1].X.ToString(CultureInfo.InvariantCulture)} {face.Plane[1].Y.ToString(CultureInfo.InvariantCulture)} {face.Plane[1].Z.ToString(CultureInfo.InvariantCulture)} ) ");
                            faceLine.Append(
                                $"( {face.Plane[2].X.ToString(CultureInfo.InvariantCulture)} {face.Plane[2].Y.ToString(CultureInfo.InvariantCulture)} {face.Plane[2].Z.ToString(CultureInfo.InvariantCulture)} ) ");

                            faceLine.Append($"{face.Texture} ");

                            faceLine.Append(
                                $"[ {face.Uaxis[0].ToString(CultureInfo.InvariantCulture)} {face.Uaxis[1].ToString(CultureInfo.InvariantCulture)} {face.Uaxis[2].ToString(CultureInfo.InvariantCulture)} {face.Uaxis[3].ToString(CultureInfo.InvariantCulture)} ] ");

                            faceLine.Append(
                                $"[ {face.Vaxis[0].ToString(CultureInfo.InvariantCulture)} {face.Vaxis[1].ToString(CultureInfo.InvariantCulture)} {face.Vaxis[2].ToString(CultureInfo.InvariantCulture)} {face.Vaxis[3].ToString(CultureInfo.InvariantCulture)} ] ");

                            faceLine.Append($"{face.Rotation.ToString(CultureInfo.InvariantCulture)} ");
                            faceLine.Append($"{face.Uscale.ToString(CultureInfo.InvariantCulture)} ");
                            faceLine.Append($"{face.Vscale.ToString(CultureInfo.InvariantCulture)}");

                            writer.WriteLine($"{faceLine}");
                        }

                        writer.WriteLine("}");
                        brushNumber++;
                    }

                    writer.WriteLine("}");
                    entityNumber++;
                }
            }
        }
        private enum ParseState
        {
            None,
            InEntity,
            InBrush
        }
    }
}