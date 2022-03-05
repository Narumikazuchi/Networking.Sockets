namespace Narumikazuchi.Networking.Sockets;

/// <summary>
/// Creates a new <see cref="IServer{TData}"/> that utilizes <see cref="Socket"/>s for communication.
/// </summary>
public static class CreateSocketServer
{
    /// <summary>
    /// Designates the specified port for the communication of clients with the server.
    /// </summary>
    /// <param name="port">The port over which the server communicates with clients.</param>
    public static IServerWithPort WithPort(in Int32 port)
    {
        __ServerBuilder builder = new()
        {
            Port = port
        };
        return builder;
    }

    /// <summary>
    /// Sets the size of the <see cref="Byte"/>[] buffer in which incoming raw messages are temporarily stored. 
    /// </summary>
    /// <param name="bufferSize">The size of the temporary buffer.</param>
    public static IServerWithBufferSize WithBufferSize(in Int32 bufferSize)
    {
        __ServerBuilder builder = new()
        {
            BufferSize = bufferSize
        };
        return builder;
    }
}