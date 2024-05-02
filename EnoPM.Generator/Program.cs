using System.Xml;
using System.Xml.Linq;
using BepInEx.AssemblyPublicizer;
using CommandLine;
using JetBrains.Annotations;

namespace EnoPM.Generator;

[UsedImplicitly]
internal class Program
{
    private static readonly List<string> IgnoredAssemblies = new()
    {
        "Assembly-CSharp-firstpass",
        "UnityEngine.AnimationModule",
        "UnityEngine.PhysicsModule",
        "System.Core",
        "System",
        "netstandard",
        "mscorlib",
        "Newtonsoft.Json",
        "System.ComponentModel.Composition",
        "System.Configuration",
        "System.Data.DataSetExtensions",
        "System.Data",
        "System.Drawing",
        "System.EnterpriseServices",
        "System.IO.Compression",
        "System.IO.Compression.FileSystem",
        "System.Net.Http",
        "System.Numerics",
        "System.Runtime",
        "System.Runtime.Serialization",
        "System.Security",
        "System.ServiceModel.Internals",
        "System.Transactions",
        "System.Xml",
        "System.Xml.Linq",
        "Newtonsoft.Json"
    };

    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(Start);
    }

    private static void Start(CommandLineOptions options)
    {
        var files = Directory.GetFiles(options.ManagedDirectoryPath);
        if (!Directory.Exists(options.OutputDirectoryPath))
        {
            Directory.CreateDirectory(options.OutputDirectoryPath);
        }
        var references = new List<XElement>();
        foreach (var filePath in files)
        {
            var file = new FileInfo(filePath);
            if (file.Extension != ".dll") continue;
            var fileName = Path.GetFileNameWithoutExtension(file.Name);
            if (IgnoredAssemblies.Contains(fileName)) continue;
            Console.WriteLine(Path.Combine(options.OutputDirectoryPath, file.Name));
            var config = new AssemblyPublicizerOptions
            {
                Strip = true
            };
#if DEBUG
            config.Strip = false;
#endif
            AssemblyPublicizer.Publicize(filePath, Path.Combine(options.OutputDirectoryPath, file.Name), config);
            references.Add(new XElement("Reference",
                    new XAttribute("Include", fileName),
                    new XElement("HintPath", $@"$(ManagedGamePath)\{file.Name}"),
                    new XElement("Private", false)
                )
            );
        }
        var document = new XDocument(
            new XElement("Project",
                new XElement("PropertyGroup",
                    new XElement("ManagedGamePath", options.OutputDirectoryPath)
                ),
                new XElement("ItemGroup", references)
            )
        );
        using var writer = XmlWriter.Create(options.XmlOutputFilePath, new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true
        });
        document.Save(writer);
    }
}