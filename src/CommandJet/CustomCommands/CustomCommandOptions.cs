namespace CommandJet.CustomCommands;

internal sealed class CustomCommandOptions
{
    public required string WorkingDirectory { get; set; }
    public string? PrivateKeyFullPath { get; set; }
    public string? RemoteAddress { get; set; }
    public string? RemoteUsername { get; set; }
    public required string ProcessName { get; set; }
    public string? Arguments { get; set; }
}
