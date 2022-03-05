namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IServerUsingByteSerializationWithDataProcessor<TMessage> :
    IServerUsingByteSerializationFinalizer<TMessage>
        where TMessage : IDeserializable<TMessage>, ISerializable
#pragma warning restore
{
    /// <summary>
    /// Sets a specific condition by which the server will decide whether to accept an incoming connection or not.
    /// </summary>
    /// <param name="acceptCondition">The condition for incoming connections.</param>
    public IServerUsingByteSerializationWithoutStrategies<TMessage> WithAcceptCondition([DisallowNull] ServerAcceptCondition<TMessage> acceptCondition);

    /// <summary>
    /// Adds the specified <see cref="ISerializationStrategy{TReturn}"/> objects to the standard strategies that are used by the internal <see cref="IByteSerializerDeserializer{TSerializable}"/>.
    /// </summary>
    /// <param name="strategies">The strategies to append to the internal serializer.</param>
    public IServerUsingByteSerializationWithoutAcceptCondition<TMessage> WithSerializationStrategies([DisallowNull] IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>> strategies);
}