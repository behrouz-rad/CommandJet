using CommandJet.Core;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;

namespace CommandJet;

[VisualStudioContribution]
internal sealed class CommandJetCommand : Command
{
    public override CommandConfiguration CommandConfiguration => new("%CommandJet.DisplayName%")
    {
        Icon = new(ImageMoniker.Custom("Icon"), IconSettings.IconAndText),
        Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        EnabledWhen = ActivationConstraint.SolutionState(SolutionState.FullyLoaded)
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {
        var shell = Extensibility.Shell();

        try
        {
            var commandJetPlugin = await new CommandJetPluginBuilder().Initialize(context, shell, cancellationToken)
                                                                      .BuildAsync();

            var commandJetDialogData = commandJetPlugin.ToCommandJetDialogData();

            using (var commandJetDialog = new CommandJetDialog(commandJetDialogData))
            {
                await shell.ShowDialogAsync(commandJetDialog, "Command Jet", cancellationToken);
            }
        }
        catch (Exception ex)
        {
            await shell.ShowPromptAsync(
                                ex.ToString(),
                                PromptOptions.OK,
                                cancellationToken);
        }
    }
}
