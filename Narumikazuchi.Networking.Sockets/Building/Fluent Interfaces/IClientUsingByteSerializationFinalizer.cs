namespace Narumikazuchi.Networking.Sockets;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591
public interface IClientUsingByteSerializationFinalizer<TMessage>
        where TMessage : IDeserializable<TMessage>, ISerializable
#pragma warning restore
{
    /// <summary>
    /// Finalizes the configuration and returns the configured <see cref="IClient{TData}"/>.
    /// </summary>
    public IClient<TMessage> Create();
}