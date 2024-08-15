// © 2024 Behrouz Rad. All rights reserved.

namespace CommandJet.Core.Exceptions;

public class CommandFailedException : Exception
{
    public CommandFailedException() { }

    public CommandFailedException(string message) : base(message) { }

    public CommandFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
