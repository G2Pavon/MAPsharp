namespace MAPsharp.Lib.Formats.vmf
{

    public class VmfNode
    {
        public string Class { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public List<VmfNode> Children { get; set; }

        public VmfNode(string @class)
        {
            Class = @class;
            Properties = new Dictionary<string, string>();
            Children = new List<VmfNode>();
        }

        public override string ToString()
        {
            return $"{Class} ({Properties.Count} properties, {Children.Count} children)";
        }
    }

    public class Document
    {
        public string FilePath { get; set; }
        public List<VmfNode> RootNodes { get; set; } = new();

        public Document(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File {path} does not exist");

            FilePath = path;
            var lines = File.ReadAllLines(path);
            int index = 0;

            while (index < lines.Length)
            {
                var node = ParseNode(lines, ref index);
                if (node != null)
                    RootNodes.Add(node);
            }
        }

        private VmfNode? ParseNode(string[] lines, ref int index)
        {
            while (index < lines.Length)
            {
                var line = lines[index].Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                {
                    index++;
                    continue;
                }

                var node = new VmfNode(line);
                index++;

                if (index < lines.Length && lines[index].Trim() == "{")
                {
                    index++;

                    while (index < lines.Length && lines[index].Trim() != "}")
                    {
                        line = lines[index].Trim();

                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                        {
                            index++;
                            continue;
                        }

                        if (line.StartsWith("\""))
                        {
                            var parts = line.Trim().Split('"', StringSplitOptions.None);
                            if (parts.Length >= 4)
                            {
                                string key = parts[1].Trim();
                                string value = parts[3].Trim();
                                node.Properties[key] = value;
                            }

                            index++;
                        }
                        else
                        {
                            var child = ParseNode(lines, ref index);
                            if (child != null)
                                node.Children.Add(child);
                        }
                    }

                    index++;
                }

                return node;
            }

            return null;
        }
    }

    public static class VmfDebugger
    {
        public static void PrintTree(VmfNode vmfNode, int indent = 0)
        {
            Console.WriteLine(new string(' ', indent * 2) + vmfNode.ToString());
            foreach (var kv in vmfNode.Properties)
            {
                Console.WriteLine(new string(' ', (indent + 1) * 2) + $"{kv.Key} = {kv.Value}");
            }

            foreach (var child in vmfNode.Children)
            {
                PrintTree(child, indent + 1);
            }
        }

        public static void DebugDocument(Document doc)
        {
            Console.WriteLine($"Debug VMF Document: {doc.FilePath}");
            foreach (var node in doc.RootNodes)
            {
                PrintTree(node, 0);
            }
        }
    }
}