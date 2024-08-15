// © 2024 Behrouz Rad. All rights reserved.

using CommandJet.Core.Models;
using FluentValidation;

namespace CommandJet.Core;
internal sealed class ShellCommandValidator : AbstractValidator<ShellCommand>
{
    public ShellCommandValidator()
    {
        RuleFor(cmd => cmd.Name)
            .NotEmpty()
            .WithMessage("Name must have a value.");

        RuleFor(cmd => cmd.ProcessName)
            .NotEmpty()
            .WithMessage("ProcessName must have a value.");

        RuleFor(cmd => cmd.IsRemote)
            .NotNull()
            .WithMessage("IsRemote must have a value.");
    }
}

internal sealed class ConfigurationValidator : AbstractValidator<ConfigurationModel>
{
    public ConfigurationValidator()
    {
        RuleForEach(config => config.ShellCommands)
            .SetValidator(new ShellCommandValidator());

        RuleFor(config => config)
            .Must(HaveValidRemoteAddressAndUsername)
            .WithMessage("RemoteAddress and RemoteUsername should have value.")
            .OverridePropertyName("RemoteAddress and RemoteUsername");
    }

    private static bool HaveValidRemoteAddressAndUsername(ConfigurationModel config)
    {
        if (Array.Exists(config.ShellCommands ?? [], cmd => cmd.IsRemote))
        {
            return !string.IsNullOrWhiteSpace(config.RemoteAddress) && !string.IsNullOrWhiteSpace(config.RemoteUsername);
        }

        return true;
    }
}
