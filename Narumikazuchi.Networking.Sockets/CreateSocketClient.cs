namespace Narumikazuchi.Networking.Sockets;

/// <summary>
/// Creates a new <see cref="IClient{TData}"/> which uses a <see cref="Socket"/> for communication.
/// </summary>
public static class CreateSocketClient
{
    /// <summary>
    /// Designates the specified port for the communication with the server.
    /// </summary>
    /// <param name="port">The port over which the client communicates with the server.</param>
    public static IClientWithPort WithPort(in Int32 port)
    {
        __ClientBuilder builder = new()
        {
            Port = port
        };
        return builder;
    }

    /// <summary>
    /// Sets the size of the <see cref="Byte"/>[] buffer in which incoming raw messages are temporarily stored. 
    /// </summary>
    /// <param name="bufferSize">The size of the temporary buffer.</param>
    public static IClientWithBufferSize WithBufferSize(in Int32 bufferSize)
    {
        __ClientBuilder builder = new()
        {
            BufferSize = bufferSize
        };
        return builder;
    }
}