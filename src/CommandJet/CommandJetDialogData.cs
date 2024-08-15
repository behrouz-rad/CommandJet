using System.Runtime.Serialization;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.Extensibility.UI;

namespace CommandJet;

[DataContract]
internal sealed class CommandJetDialogData : NotifyPropertyChangedObject
{
    public ShellExtensibility? Shell { get; set; }
    public string? PrivateKeyFullPath { get; set; }
    public required string MainProjectFullPath { get; set; }
    public string? RemoteAddress { get; set; }
    public string? RemoteUsername { get; set; }

    [DataMember]
    public ObservableList<CommandResult>? CommandResults { get; set; } = [];

    [DataMember]
    public ObservableList<ShellCommandData>? ShellCommands { get; set; } = [];
}

[DataContract]
internal sealed class CommandResult : NotifyPropertyChangedObject
{
    private string? _name;
    [DataMember]
    public string? Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
        }
    }
}

[DataContract]
internal sealed class ShellCommandData
{
    [DataMember]
    public required string Name { get; set; }

    [DataMember]
    public required string ProcessName { get; set; }

    [DataMember]
    public string? Arguments { get; set; }

    [DataMember]
    public bool IsRemote { get; set; }
}
