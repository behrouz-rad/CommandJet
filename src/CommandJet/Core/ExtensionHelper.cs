using System.Diagnostics;
using CommandJet.Core.Exceptions;
using CommandJet.Core.Models;
using CommandJet.CustomCommands;
using Microsoft.VisualStudio.Extensibility.UI;
using Renci.SshNet;

namespace CommandJet.Core;

internal static class ExtensionHelper
{
    public static async Task<string> RunLocalProcessAsync(ProcessModel processModel, CancellationToken cancellationToken = default)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = processModel.ProcessName,
            Arguments = processModel.Arguments,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            WorkingDirectory = processModel.WorkingDirectory,
            CreateNoWindow = true
        };

        var process = Process.Start(processStartInfo) ?? throw new InvalidOperationException($"Failed to start process {processModel.ProcessName}.");
        await process.WaitForExitAsync(cancellationToken);

        int exitCode = process.ExitCode;
        if (exitCode != 0)
        {
            string standardErrorText = await process.StandardError.ReadToEndAsync(cancellationToken);
            string standardOutputText = await process.StandardOutput.ReadToEndAsync(cancellationToken);

            throw new InvalidOperationException($"Failed to run the command '{processModel.ProcessName} {processModel.Arguments}'. Standard error: {standardErrorText}. Standard output: {standardOutputText}");
        }

        return await process.StandardOutput.ReadToEndAsync(cancellationToken);
    }

    public static async Task<string> RunRemoteProcessAsync(ProcessModel processModel, CancellationToken cancellationToken = default)
    {
        bool isCustomCommand = CustomCommandsMapping.HasCommand(processModel.ProcessName);
        if (isCustomCommand)
        {
            var customCommandInstance = CustomCommandsMapping.CreateCustomCommand(processModel.ProcessName);

            return await customCommandInstance.RunCommandAsync(new CustomCommandOptions()
            {
                WorkingDirectory = processModel.WorkingDirectory,
                ProcessName = processModel.ProcessName,
                Arguments = processModel.Arguments,
                PrivateKeyFullPath = processModel.PrivateKeyFullPath,
                RemoteAddress = processModel.RemoteAddress,
                RemoteUsername = processModel.RemoteUsername
            }, cancellationToken);
        }

        var conn = new ConnectionInfo(
            processModel.RemoteAddress,
            processModel.RemoteUsername,
               new PrivateKeyAuthenticationMethod(processModel.RemoteUsername, new PrivateKeyFile(processModel.PrivateKeyFullPath, ""))
            )
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        using (var client = new SshClient(conn))
        {
            await client.ConnectAsync(cancellationToken);

            SshCommand sshCommand = client.RunCommand($"{processModel.ProcessName} {processModel.Arguments}");
            if (string.IsNullOrWhiteSpace(sshCommand.Error))
            {
                return sshCommand.Result;
            }

            throw new CommandFailedException(sshCommand.Error);
        }
    }

    public static Task<string> RunProcessAsync(ProcessModel processModel, CancellationToken cancellationToken = default)
    {
        return processModel.IsRemote ? RunRemoteProcessAsync(processModel, cancellationToken) :
                                       RunLocalProcessAsync(processModel, cancellationToken);
    }

    public static string GetAbsolutePath(string path, string referencePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(referencePath);

        string referenceDirectory;

        if (Directory.Exists(referencePath))
        {
            // referencePath is a directory
            referenceDirectory = referencePath;
        }
        else
        {
            // referencePath is a file
            referenceDirectory = Path.GetDirectoryName(referencePath) ?? "";
        }

        // Check if the target path is already an absolute path
        if (Path.IsPathRooted(path))
        {
            return Path.GetFullPath(path);
        }

        // Combine the relative target path with the reference directory
        return Path.GetFullPath(Path.Combine(referenceDirectory, path));
    }

    public static string GetPrivateKeyFullPath(string privateKeyPath, string configurationFilePath)
    {
        return GetAbsolutePath(privateKeyPath, configurationFilePath);
    }

    public static CommandJetDialogData ToCommandJetDialogData(this CommandJetPlugin commandJetPlugin)
    {
        return new CommandJetDialogData
        {
            Shell = commandJetPlugin.Shell,
            PrivateKeyFullPath = commandJetPlugin.PrivateKeyFullPath,
            MainProjectFullPath = commandJetPlugin.MainProjectFullPath,
            RemoteAddress = commandJetPlugin.ConfigurationModel?.RemoteAddress,
            RemoteUsername = commandJetPlugin.ConfigurationModel?.RemoteUsername,
            ShellCommands = new ObservableList<ShellCommandData>(commandJetPlugin.ConfigurationModel?.ShellCommands?.Select(c =>
                                new ShellCommandData
                                {
                                    Name = c.Name,
                                    ProcessName = c.ProcessName,
                                    Arguments = c.Arguments,
                                    IsRemote = c.IsRemote,
                                }) ?? [])
        };
    }
}
