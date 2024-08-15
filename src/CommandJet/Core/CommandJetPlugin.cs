using CommandJet.Core.Models;
using Microsoft.VisualStudio.Extensibility.Shell;

namespace CommandJet.Core;

internal sealed class CommandJetPlugin
{
    public ShellExtensibility? Shell { get; set; }
    public string? PrivateKeyFullPath { get; set; }
    public required string MainProjectFullPath { get; set; }
    public ConfigurationModel? ConfigurationModel { get; set; }
}
