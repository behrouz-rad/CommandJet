namespace CommandJet.CustomCommands;

internal interface ICustomCommand
{
    string Name { get; }
    Task<string> RunCommandAsync(CustomCommandOptions options, CancellationToken cancellationToken);
}
