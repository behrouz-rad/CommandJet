using CommandJet.Core;
using CommandJet.Core.Models;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.Extensibility.UI;

namespace CommandJet;

internal sealed class CommandJetDialog : RemoteUserControl
{
    private readonly CommandJetDialogData _dataContext;

    public CommandJetDialog(CommandJetDialogData dataContext) : base(dataContext: dataContext)
    {
        ArgumentNullException.ThrowIfNull(nameof(dataContext));

        _dataContext = dataContext;
    }

    public override async Task ControlLoadedAsync(CancellationToken cancellationToken)
    {
        await base.ControlLoadedAsync(cancellationToken);

        foreach (var command in _dataContext.ShellCommands ?? [])
        {
            var commandResult = new CommandResult() { Name = $"{command.Name}... " };
            _dataContext.CommandResults?.Add(commandResult);

            try
            {
                await ExtensionHelper.RunProcessAsync(new ProcessModel()
                {
                    ProcessName = command.ProcessName,
                    Arguments = command.Arguments,
                    WorkingDirectory = _dataContext.MainProjectFullPath,
                    IsRemote = command.IsRemote,
                    PrivateKeyFullPath = _dataContext.PrivateKeyFullPath,
                    RemoteAddress = _dataContext.RemoteAddress,
                    RemoteUsername = _dataContext.RemoteUsername
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                commandResult.Name += "Failed";

                await _dataContext.Shell!.ShowPromptAsync(
                                ex.ToString(),
                                PromptOptions.OK,
                                cancellationToken);

                return;
            }

            commandResult.Name += "Done";
        }

        _dataContext.CommandResults?.Add(new CommandResult() { Name = "Done" });
    }
}
