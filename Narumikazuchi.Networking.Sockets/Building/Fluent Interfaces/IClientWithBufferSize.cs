namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IClientWithBufferSize
#pragma warning restore
{
    /// <summary>
    /// Designates the specified port for the communication with the server.
    /// </summary>
    /// <param name="port">The port over which the client communicates with the server.</param>
    public IClientWithoutSerialization WithPort(in Int32 port);
}