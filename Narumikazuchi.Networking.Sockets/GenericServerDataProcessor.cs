namespace Narumikazuchi.Networking.Sockets;

/// <summary>
/// Provides the blueprint for data processing of an <see cref="IServer{TData}"/>.
/// </summary>
// Non-Public
public abstract partial class ServerDataProcessor<TMessage>
    where TMessage : IDeserializable<TMessage>, ISerializable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerDataProcessor{TMessage}"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    protected ServerDataProcessor([DisallowNull] IServer<TMessage> server!!)
    {
        m_Server = server;
        if (m_Server.DataProcessor != this)
        {
            m_Server.DataProcessor = this;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IServer<TMessage> m_Server;
}

// IServerDataProcessor<TMessage>
partial class ServerDataProcessor<TMessage> : IServerDataProcessor<TMessage>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public abstract void ProcessReceivedData([DisallowNull] TMessage data,
                                             in Guid fromClient);

    /// <summary>
    /// Disconnects the <see cref="IClient{TMessage}"/> with the specified <see cref="Guid"/> from the <see cref="IServer{TMessage}"/> instance.
    /// </summary>
    /// <param name="client">The client to disconnect.</param>
    /// <exception cref="KeyNotFoundException"/>
    public void DisconnectClient(in Guid client) =>
        this.Server
            .Disconnect(client);

    /// <summary>
    /// Gets or sets the <see cref="IServer{TMessage}"/> associated with this processor.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    [NotNull]
    public IServer<TMessage> Server
    {
        get => m_Server;
        set
        {
            ExceptionHelpers.ThrowIfArgumentNull(value);

            m_Server = value;
            if (m_Server.DataProcessor != this)
            {
                m_Server.DataProcessor = this;
            }
        }
    }
}