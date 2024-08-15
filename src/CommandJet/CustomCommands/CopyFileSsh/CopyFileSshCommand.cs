using CommandJet.Core;
using Renci.SshNet;

namespace CommandJet.CustomCommands.CopyFileSsh;

/// <summary>
/// Expected command: copy-file-ssh -s C:/LocalSourceFolder -d /C:/RemoteDestinationFolder
/// </summary>
internal sealed class CopyFileSshCommand : CustomCommandBase<CopyFileSshArguments>
{
    public override string Name { get => "copy-file-ssh"; }

    protected async override Task<string> RunCommandInternalAsync(CustomCommandOptions options, CopyFileSshArguments arguments, CancellationToken cancellationToken)
    {
        var connInfo = GetConnectionInfo(options);

        using (var client = new SftpClient(connInfo))
        {
            await client.ConnectAsync(cancellationToken);

            string fileFullPath = ExtensionHelper.GetAbsolutePath(arguments.LocalPath, options.WorkingDirectory);

            await using (var fileStream = new FileStream(fileFullPath, FileMode.Open))
            {
                client.BufferSize = 4 * 1024; // Bypass Payload error large files.

                client.UploadFile(fileStream, $"{arguments.RemotePath}", canOverride: true);
            }
        }

        return "OK";
    }
}
