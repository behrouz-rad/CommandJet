namespace CommandJet.Core.Models;

internal sealed class ProcessModel
{
    public required string ProcessName { get; set; }
    public string? Arguments { get; set; }
    public required string WorkingDirectory { get; set; }
    public bool IsRemote { get; set; }
    public string? PrivateKeyFullPath { get; set; }
    public string? RemoteAddress { get; set; }
    public string? RemoteUsername { get; set; }
}
