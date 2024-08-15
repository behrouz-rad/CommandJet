using CommandLine;

namespace CommandJet.CustomCommands.CopyFileSsh;

internal sealed class CopyFileSshArguments : CustomCommandArgumentsBase
{
    [Option('s', "source", Required = true, HelpText = "The local path")]
    public required string LocalPath { get; set; }

    [Option('d', "destination", Required = true, HelpText = "The remote path")]
    public required string RemotePath { get; set; }
}
