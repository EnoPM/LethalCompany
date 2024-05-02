using CommandLine;
using JetBrains.Annotations;

namespace EnoPM.Generator;

[UsedImplicitly]
public class CommandLineOptions
{
    [Option('m', "managed", Required = true, HelpText = "Define managed directory path")]
    public string ManagedDirectoryPath { get; set; }
    
    [Option('o', "output", Required = true, HelpText = "Define output directory path")]
    public string OutputDirectoryPath { get; set; }
    
    [Option('x', "xml", Required = true, HelpText = "XML output file path")]
    public string XmlOutputFilePath { get; set; }
}