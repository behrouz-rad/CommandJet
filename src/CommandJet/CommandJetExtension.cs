using Microsoft.VisualStudio.Extensibility;

namespace CommandJet;

[VisualStudioContribution]
internal class CommandJetExtension : Extension
{
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "CommandJet.ecc139c5-ba47-428d-ab14-e1bc5e1fd6d8",
                version: ExtensionAssemblyVersion,
                publisherName: "Behrouz Rad",
                displayName: "Command Jet",
                description: "One-click running of your commands from Visual Studio directly.")
        {
            InstallationTargetVersion = "[17.10, 18.0)",
            Icon = "Images\\Icon.32.32.png",
            PreviewImage = "Images\\Icon.256.256.png",
            License = "Resources\\LICENSE",
            MoreInfo = "https://github.com/behrouz-rad/CommandJet",
            Preview = false,
            Tags = ["command", "run"]
        }
    };
}
