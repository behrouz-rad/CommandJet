using System.IO.Compression;
using CommandJet.Core.Exceptions;
using Renci.SshNet;

namespace CommandJet.CustomCommands.CopyFolderSsh;

/// <summary>
/// Expected command: copy-folder-ssh -s C:/LocalSourceFolder -d /C:/RemoteDestinationFolder
/// </summary>
internal sealed class CopyFolderSshCommand : CustomCommandBase<CopyFolderSshArguments>
{
    public override string Name { get => "copy-folder-ssh"; }

    protected async override Task<string> RunCommandInternalAsync(CustomCommandOptions options, CopyFolderSshArguments arguments, CancellationToken cancellationToken)
    {
        var connInfo = GetConnectionInfo(options);

        string zipFileFullPath;
        using (var client = new SftpClient(connInfo))
        {
            await client.ConnectAsync(cancellationToken);

            var tempFolderResult = CreateTempFolder();

            var zipFileName = $"{Guid.NewGuid()}.zip";
            zipFileFullPath = Path.Combine(tempFolderResult, zipFileName);

            ZipFile.CreateFromDirectory(arguments.LocalPath, zipFileFullPath);

            await using (var fileStream = new FileStream(zipFileFullPath, FileMode.Open))
            {
                client.BufferSize = 4 * 1024; // Bypass Payload error large files.

                client.UploadFile(fileStream, $"{arguments.RemotePath}", canOverride: true);
            }
        }

        using (var client = new SshClient(connInfo))
        {
            await client.ConnectAsync(cancellationToken);
            SshCommand runCommandResult = client.RunCommand($"powershell Expand-Archive '{zipFileFullPath}' -DestinationPath '{arguments.RemotePath}' -Force");

            if (!string.IsNullOrWhiteSpace(runCommandResult.Error))
            {
                throw new CommandFailedException(runCommandResult.Error);
            }
        }

        return "OK";
    }

    private static string CreateTempFolder()
    {
        string tempPath = Path.GetTempPath();
        string folderName = Path.GetRandomFileName();
        string tempFolderPath = Path.Combine(tempPath, folderName);

        DirectoryInfo dirInfo = Directory.CreateDirectory(tempFolderPath);

        return dirInfo.FullName;
    }
}
