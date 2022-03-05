namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IClientUsingByteSerializationWithoutProcessor<TMessage> :
    IClientUsingByteSerializationFinalizer<TMessage>
        where TMessage : IDeserializable<TMessage>, ISerializable
#pragma warning restore
{
    /// <summary>
    /// Designates the specified <see cref="IClientDataProcessor{TData}"/> to the client.
    /// </summary>
    /// <param name="processor">The data processor that will handle incoming messages.</param>
    public IClientUsingByteSerializationFinalizer<TMessage> WithDataProcessor([DisallowNull] IClientDataProcessor<TMessage> processor);
}