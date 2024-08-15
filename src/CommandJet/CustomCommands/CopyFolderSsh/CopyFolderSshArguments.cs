using CommandLine;

namespace CommandJet.CustomCommands.CopyFolderSsh;

internal sealed class CopyFolderSshArguments : CustomCommandArgumentsBase
{
    [Option('s', "source", Required = true, HelpText = "The local path")]
    public required string LocalPath { get; set; }

    [Option('d', "destination", Required = true, HelpText = "The remote path")]
    public required string RemotePath { get; set; }
}
