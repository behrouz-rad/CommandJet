using CommandJet.Core.Exceptions;
using CommandLine;
using Renci.SshNet;

namespace CommandJet.CustomCommands;

internal abstract class CustomCommandBase<TArguments> : ICustomCommand where TArguments : CustomCommandArgumentsBase
{
    public abstract string Name { get; }

    protected abstract Task<string> RunCommandInternalAsync(CustomCommandOptions options, TArguments arguments, CancellationToken cancellationToken);

    public Task<string> RunCommandAsync(CustomCommandOptions options, CancellationToken cancellationToken)
    {
        return RunCommandAsync(options, RunCommandInternalAsync, cancellationToken);
    }

    private async Task<string> RunCommandAsync(CustomCommandOptions options,
                                               Func<CustomCommandOptions, TArguments, CancellationToken, Task<string>> action,
                                               CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        bool isStartedWithCommandName = options.ProcessName.Equals(Name, StringComparison.OrdinalIgnoreCase);
        if (!isStartedWithCommandName)
        {
            throw new CommandNotFoundException($"The command doesn't start with {Name}");
        }

        var commandArguments = (options.Arguments ?? "").Trim().SplitArgs();
        var parsedResult = Parser.Default.ParseArguments<TArguments>(commandArguments);
        string result = "";

        await parsedResult.WithParsedAsync(async arguments =>
        {
            result = await action(options, arguments, cancellationToken);
        });

        await parsedResult.WithNotParsedAsync(invalidArgs =>
        {
            string errors = string.Concat(ParseErrors(invalidArgs).Select(arg => $"- {arg}\n"));

            throw new InvalidArgumentException($"Arguments are invalid:\n{errors}");
        });

        return result;
    }

    protected static ConnectionInfo GetConnectionInfo(CustomCommandOptions options)
    {
        if (!File.Exists(options.PrivateKeyFullPath))
        {
            throw new FileNotFoundException($"Priavte key in {options.PrivateKeyFullPath} was not found.");
        }

        return new ConnectionInfo(
                   options.RemoteAddress,
                   options.RemoteUsername,
                   new PrivateKeyAuthenticationMethod(options.RemoteUsername, new PrivateKeyFile(options.PrivateKeyFullPath, "")))
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
    }

    private static List<string> ParseErrors(IEnumerable<CommandLine.Error> errors)
    {
        List<string> errorMessages = [];
        foreach (CommandLine.Error error in errors)
        {
            switch (error)
            {
                case UnknownOptionError unknownOptionError:
                    errorMessages.Add($"Unknown option: {unknownOptionError.Token}");
                    break;
                case RepeatedOptionError repeatedOptionError:
                    errorMessages.Add($"Repeated option: {repeatedOptionError.NameInfo.NameText}");
                    break;
                default:
                    errorMessages.Add($"{error.Tag}");
                    break;
            }
        }

        return errorMessages;
    }
}
