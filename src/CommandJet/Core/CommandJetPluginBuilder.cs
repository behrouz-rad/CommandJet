using System.Text.Json;
using CommandJet.Core.Exceptions;
using CommandJet.Core.Models;
using FluentResults;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.ProjectSystem.Query;

namespace CommandJet.Core;

internal sealed class CommandJetPluginBuilder
{
    readonly private CommandJetPlugin _commandJetPlugin = new() { MainProjectFullPath = "" };
    private IClientContext? _clientContext;
    private const string _configuration_file_name = "commandjet.settings.json";
    private CancellationToken _cancellationToken = CancellationToken.None;

    public CommandJetPluginBuilder Initialize(IClientContext clientContext,
                                              ShellExtensibility shell,
                                              CancellationToken cancellationToken)
    {
        _clientContext = clientContext;
        _commandJetPlugin.Shell = shell;
        _cancellationToken = cancellationToken;

        return this;
    }

    // TODO: use Fluent Interface Pattern
    public async Task<CommandJetPlugin> BuildAsync()
    {
        await SetMainProjectFullPathAsync();
        await SetConfigurationAsync(_commandJetPlugin.MainProjectFullPath);

        var validationResult = await ValidateConfigurationAsync(_commandJetPlugin.ConfigurationModel ?? new ConfigurationModel());
        if (validationResult.IsFailed)
        {
            string errors = string.Join(Environment.NewLine, validationResult.Errors.Select(err => err));

            throw new InvalidConfigurationException(errors);
        }

        SetPrivateKeyFullPath();

        return _commandJetPlugin;
    }

    private async Task SetMainProjectFullPathAsync()
    {
        var queryResultsMainPath = await _clientContext!.Extensibility.Workspaces().QueryProjectsAsync(
        query => query.Get(project => project.Files)
                        .With(project => project.Path)
                        .Where(file => file.FileName == _configuration_file_name), _cancellationToken);

        if (queryResultsMainPath.Any())
        {
            _commandJetPlugin.MainProjectFullPath = Path.GetDirectoryName(queryResultsMainPath.FirstOrDefault()?.Path) ?? throw new InvalidOperationException("The main project path cannot be set");

            return;
        }

        throw new FileNotFoundException($"The file {_configuration_file_name} was not found.");
    }

    private async Task SetConfigurationAsync(string mainProjectFullPath)
    {
        var mainProjectPath = mainProjectFullPath;

        var configurationFilePath = Path.Combine(mainProjectPath!, _configuration_file_name);

        var configurationFileContent = await File.ReadAllTextAsync(configurationFilePath, _cancellationToken);

        _commandJetPlugin.ConfigurationModel = JsonSerializer.Deserialize<ConfigurationModel>(configurationFileContent) ?? new ConfigurationModel();
    }

    private async Task<Result<List<string>>> ValidateConfigurationAsync(ConfigurationModel configurationModel)
    {
        var validator = new ConfigurationValidator();
        var results = await validator.ValidateAsync(configurationModel, _cancellationToken);

        var errors = new List<string>();
        if (!results.IsValid)
        {
            foreach (var failure in results.Errors)
            {
                errors.Add($"Property {failure.PropertyName} failed validation. Error: {failure.ErrorMessage}");
            }
        }

        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok();
    }

    private void SetPrivateKeyFullPath()
    {
        var configurationFileFullPath = Path.Combine(_commandJetPlugin.MainProjectFullPath!, _configuration_file_name);

        _commandJetPlugin.PrivateKeyFullPath = ExtensionHelper.GetPrivateKeyFullPath(_commandJetPlugin.ConfigurationModel!.PrivateKeyPath!, configurationFileFullPath);
    }
}
