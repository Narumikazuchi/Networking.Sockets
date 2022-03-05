namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IClientUsingByteArrays :
    IClientUsingByteArraysFinalizer
#pragma warning restore
{
    /// <summary>
    /// Designates the specified <see cref="IClientDataProcessor{TData}"/> to the client.
    /// </summary>
    /// <param name="processor">The data processor that will handle incoming messages.</param>
    public IClientUsingByteArraysFinalizer WithDataProcessor([DisallowNull] IClientDataProcessor<Byte[]> processor);
}