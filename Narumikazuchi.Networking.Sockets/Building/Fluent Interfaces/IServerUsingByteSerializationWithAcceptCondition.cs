namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IServerUsingByteSerializationWithAcceptCondition<TMessage> :
    IServerUsingByteSerializationFinalizer<TMessage>
        where TMessage : IDeserializable<TMessage>, ISerializable
#pragma warning restore
{
    /// <summary>
    /// Designates the specified <see cref="IServerDataProcessor{TData}"/> to the server.
    /// </summary>
    /// <param name="processor">The data processor that will handle incoming messages.</param>
    public IServerUsingByteSerializationWithoutStrategies<TMessage> WithDataProcessor([DisallowNull] IServerDataProcessor<TMessage> processor);

    /// <summary>
    /// Adds the specified <see cref="ISerializationStrategy{TReturn}"/> objects to the standard strategies that are used by the internal <see cref="IByteSerializerDeserializer{TSerializable}"/>.
    /// </summary>
    /// <param name="strategies">The strategies to append to the internal serializer.</param>
    public IServerUsingByteSerializationWithoutDataProcessor<TMessage> WithSerializationStrategies([DisallowNull] IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>> strategies);
}