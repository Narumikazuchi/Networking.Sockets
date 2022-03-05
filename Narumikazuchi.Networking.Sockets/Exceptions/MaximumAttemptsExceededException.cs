namespace Narumikazuchi.Networking.Sockets;

/// <summary>
/// Represents errors which happen when connecting two endpoints.
/// </summary>
public sealed partial class MaximumAttemptsExceededException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaximumAttemptsExceededException"/> class.
    /// </summary>
    public MaximumAttemptsExceededException() :
        base(MESSAGE)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaximumAttemptsExceededException"/> class.
    /// </summary>
    public MaximumAttemptsExceededException([AllowNull] String? message) :
        base(String.Format("{0} - {1}",
                           MESSAGE,
                           message))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaximumAttemptsExceededException"/> class.
    /// </summary>
    public MaximumAttemptsExceededException([AllowNull] String? message,
                                            [AllowNull] Exception? inner) :
        base(String.Format("{0} - {1}",
                           MESSAGE,
                           message),
             inner)
    { }
}

// Non-Public
partial class MaximumAttemptsExceededException
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const String MESSAGE = "Maximum number of attempts when connecting to endpoint exceeded.";
}