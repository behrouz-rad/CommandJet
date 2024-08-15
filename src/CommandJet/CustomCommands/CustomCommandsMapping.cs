using System.Collections.Immutable;
using CommandJet.Core.Exceptions;
using CommandJet.CustomCommands.CopyFileSsh;
using CommandJet.CustomCommands.CopyFolderSsh;

namespace CommandJet.CustomCommands;

internal static class CustomCommandsMapping
{
    private static readonly Dictionary<string, ICustomCommand> _customCommandImplementers = new(StringComparer.InvariantCultureIgnoreCase)
    {
        {
            "copy-file-ssh",
            new CopyFileSshCommand()
        },
        {
            "copy-folder-ssh",
            new CopyFolderSshCommand()
        }
    };

    private static ImmutableDictionary<string, ICustomCommand> CustomCommandImplementers
    {
        get
        {
            return _customCommandImplementers.ToImmutableDictionary();
        }
    }

    public static bool HasCommand(string commandName)
    {
        return CustomCommandImplementers.Keys.Any(key => key.Equals(commandName, StringComparison.OrdinalIgnoreCase));
    }

    public static ICustomCommand CreateCustomCommand(string commandName)
    {
        if (CustomCommandImplementers.TryGetValue(commandName, out ICustomCommand? customCommand))
        {
            var customCommandInstance = customCommand.GetType();

            return (ICustomCommand)Activator.CreateInstance(customCommandInstance)!;
        }

        throw new CommandNotFoundException($"Command '{commandName}' not found.");
    }
}
