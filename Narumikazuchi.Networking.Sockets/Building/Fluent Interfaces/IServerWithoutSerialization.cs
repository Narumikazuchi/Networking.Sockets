namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IServerWithoutSerialization
#pragma warning restore
{
    /// <summary>
    /// Setups the server to only handle raw data, that is <see cref="Byte"/>[] arrays.
    /// </summary>
    public IServerUsingByteArrays UsingByteArrays();

    /// <summary>
    /// Setups the server to handle a certain type <typeparamref name="TMessage"/> for messages.
    /// </summary>
    public IServerUsingByteSerialization<TMessage> UsingByteSerialization<TMessage>()
        where TMessage : IDeserializable<TMessage>, ISerializable;
}