namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IClientUsingByteSerialization<TMessage> :
    IClientUsingByteSerializationFinalizer<TMessage>
        where TMessage : IDeserializable<TMessage>, ISerializable
#pragma warning restore
{
    /// <summary>
    /// Designates the specified <see cref="IClientDataProcessor{TData}"/> to the client.
    /// </summary>
    /// <param name="processor">The data processor that will handle incoming messages.</param>
    public IClientUsingByteSerializationWithoutStrategies<TMessage> WithDataProcessor([DisallowNull] IClientDataProcessor<TMessage> processor);

    /// <summary>
    /// Adds the specified <see cref="ISerializationStrategy{TReturn}"/> objects to the standard strategies that are used by the internal <see cref="IByteSerializerDeserializer{TSerializable}"/>.
    /// </summary>
    /// <param name="strategies">The strategies to append to the internal serializer.</param>
    public IClientUsingByteSerializationWithoutProcessor<TMessage> WithSerializationStrategies([DisallowNull] IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>> strategies);
}