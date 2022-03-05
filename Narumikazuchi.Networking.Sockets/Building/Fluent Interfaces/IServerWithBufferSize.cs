namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IServerWithBufferSize
#pragma warning restore
{
    /// <summary>
    /// Designates the specified port for the communication of clients with the server.
    /// </summary>
    /// <param name="port">The port over which the server communicates with clients.</param>
    public IServerWithoutSerialization WithPort(in Int32 port);
}