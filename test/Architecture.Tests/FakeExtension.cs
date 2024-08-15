// © 2024 Behrouz Rad. All rights reserved.

using Microsoft.VisualStudio.Extensibility;

namespace Architecture.Tests;

[VisualStudioContribution]
internal sealed class FakeExtension : Extension
{
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "CommandJet.d419c615-df06-4fd8-9383-b89def27b4b3",
                version: ExtensionAssemblyVersion,
                publisherName: "Behrouz Rad",
                displayName: "Command Jet",
                description: "One-click running of your commands from Visual Studio directly."),
    };
}
