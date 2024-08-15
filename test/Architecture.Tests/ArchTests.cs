// © 2024 Behrouz Rad. All rights reserved.

using CommandJet;
using CommandJet.CustomCommands;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;
using Xunit.Abstractions;

namespace Architecture.Tests;

public sealed class ArchTests(ITestOutputHelper output)
{
    // Arrange
    private static readonly Assembly _commandJetAssembly = typeof(CommandJetExtension).Assembly;

    [Fact]
    public void CustomCommands_Should_HaveNameEndingWith_Command()
    {
        // Act
        var testResult = Types
            .InAssembly(_commandJetAssembly)
            .That()
            .Inherit(typeof(CustomCommandBase<>))
            .Should()
            .HaveNameEndingWith("Command", StringComparison.Ordinal)
            .GetResult();

        testResult.WriteFailedTests(output);

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CustomCommands_Should_Be_Sealed()
    {
        // Act
        var testResult = Types
            .InAssembly(_commandJetAssembly)
            .That()
            .Inherit(typeof(CustomCommandBase<>))
            .Should()
            .BeSealed()
            .GetResult();

        testResult.WriteFailedTests(output);

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CustomCommands_Should_Have_A_Name()
    {
        // Act
        var customCommands = Types
            .InAssembly(_commandJetAssembly)
            .That()
            .Inherit(typeof(CustomCommandBase<>))
            .GetTypes();

        var failingTypes = new List<Type>();
        foreach (var customCommand in customCommands)
        {
            PropertyInfo? propertyInfo = customCommand.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo is not null)
            {
                object? customCommandBase = Activator.CreateInstance(customCommand);
                object? name = propertyInfo.GetValue(customCommandBase);
                string nameValue = name as string ?? string.Empty;

                if (string.IsNullOrWhiteSpace(nameValue))
                {
                    failingTypes.Add(customCommand);
                }
            }
        }

        failingTypes.WriteFailedTests(output);

        // Assert
        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void CustomCommands_Should_Have_A_Mapping()
    {
        // Act
        var customCommands = Types
            .InAssembly(_commandJetAssembly)
            .That()
            .Inherit(typeof(CustomCommandBase<>))
            .GetTypes();

        var failingTypes = new List<Type>();
        foreach (var customCommand in customCommands)
        {
            PropertyInfo? propertyInfo = customCommand.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo is not null)
            {
                object? customCommandBase = Activator.CreateInstance(customCommand);
                object? name = propertyInfo.GetValue(customCommandBase);
                string nameValue = name as string ?? string.Empty;

                if (!CustomCommandsMapping.HasCommand(nameValue))
                {
                    failingTypes.Add(customCommand);
                }
            }
        }

        failingTypes.WriteFailedTests(output);

        // Assert
        failingTypes.Should().BeEmpty();
    }
}
