namespace CommandJet.Core.Models;

internal sealed class ConfigurationModel
{
    public string? PrivateKeyPath { get; set; }
    public string? RemoteAddress { get; set; }
    public string? RemoteUsername { get; set; }
    public ShellCommand[]? ShellCommands { get; set; }
}

internal sealed class ShellCommand
{
    public required string Name { get; set; }
    public required string ProcessName { get; set; }
    public string? Arguments { get; set; }
    public bool IsRemote { get; set; }
}
