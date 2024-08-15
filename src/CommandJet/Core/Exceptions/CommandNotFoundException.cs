// © 2024 Behrouz Rad. All rights reserved.

namespace CommandJet.Core.Exceptions;

public class CommandNotFoundException : Exception
{
    public CommandNotFoundException() { }

    public CommandNotFoundException(string message) : base(message) { }

    public CommandNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
