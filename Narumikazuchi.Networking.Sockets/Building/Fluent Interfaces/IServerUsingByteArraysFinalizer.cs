namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IServerUsingByteArraysFinalizer
#pragma warning restore
{
    /// <summary>
    /// Finalizes the configuration and returns the configured <see cref="IServer{TData}"/>.
    /// </summary>
    public IServer<Byte[]> Create();
}