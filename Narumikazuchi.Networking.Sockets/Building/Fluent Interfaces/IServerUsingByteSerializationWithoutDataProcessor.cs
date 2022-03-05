namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IServerUsingByteSerializationWithoutDataProcessor<TMessage> :
    IServerUsingByteSerializationFinalizer<TMessage>
        where TMessage : IDeserializable<TMessage>, ISerializable
#pragma warning restore
{
    /// <summary>
    /// Designates the specified <see cref="IServerDataProcessor{TData}"/> to the server.
    /// </summary>
    /// <param name="processor">The data processor that will handle incoming messages.</param>
    public IServerUsingByteSerializationFinalizer<TMessage> WithDataProcessor([DisallowNull] IServerDataProcessor<TMessage> processor);
}