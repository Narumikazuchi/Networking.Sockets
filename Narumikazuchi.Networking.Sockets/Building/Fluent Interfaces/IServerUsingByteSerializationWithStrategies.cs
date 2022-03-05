namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IServerUsingByteSerializationWithStrategies<TMessage> :
    IServerUsingByteSerializationFinalizer<TMessage>
        where TMessage : IDeserializable<TMessage>, ISerializable
#pragma warning restore
{
    /// <summary>
    /// Sets a specific condition by which the server will decide whether to accept an incoming connection or not.
    /// </summary>
    /// <param name="acceptCondition">The condition for incoming connections.</param>
    public IServerUsingByteSerializationWithoutDataProcessor<TMessage> WithAcceptCondition([DisallowNull] ServerAcceptCondition<TMessage> acceptCondition);

    /// <summary>
    /// Designates the specified <see cref="IServerDataProcessor{TData}"/> to the server.
    /// </summary>
    /// <param name="processor">The data processor that will handle incoming messages.</param>
    public IServerUsingByteSerializationWithoutAcceptCondition<TMessage> WithDataProcessor([DisallowNull] IServerDataProcessor<TMessage> processor);
}