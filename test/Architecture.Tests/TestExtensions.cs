// © 2024 Behrouz Rad. All rights reserved.

using NetArchTest.Rules;
using Xunit.Abstractions;

namespace Architecture.Tests;
internal static class TestExtensions
{
    public static void WriteFailedTests(this TestResult testResult, ITestOutputHelper output)
    {
        if (!testResult.IsSuccessful)
        {
            var failingTypes = testResult.FailingTypes;
            foreach (var type in failingTypes)
            {
                output.WriteLine($"Type {type.FullName} caused the test to fail.");
            }
        }
    }

    public static void WriteFailedTests(this List<Type> types, ITestOutputHelper output)
    {
        foreach (var type in types)
        {
            output.WriteLine($"Type {type.FullName} caused the test to fail.");
        }
    }
}
