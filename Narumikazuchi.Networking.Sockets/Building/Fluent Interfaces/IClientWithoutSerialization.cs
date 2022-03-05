namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IClientWithoutSerialization
#pragma warning restore
{
    /// <summary>
    /// Setups the client to only handle raw data, that is <see cref="Byte"/>[] arrays.
    /// </summary>
    public IClientUsingByteArrays UsingByteArrays();

    /// <summary>
    /// Setups the client to handle a certain type <typeparamref name="TMessage"/> for messages.
    /// </summary>
    public IClientUsingByteSerialization<TMessage> UsingByteSerialization<TMessage>()
        where TMessage : IDeserializable<TMessage>, ISerializable;
}