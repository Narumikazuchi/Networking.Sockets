namespace Narumikazuchi.Networking.Sockets;

/// <summary>
/// Provides the blueprint for data processing of an <see cref="IClient{TData}"/>.
/// </summary>
// Non-Public
public abstract partial class ClientDataProcessor<TMessage>
    where TMessage : IDeserializable<TMessage>, ISerializable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientDataProcessor{TMessage}"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    protected ClientDataProcessor([DisallowNull] IClient<TMessage> client!!)
    {
        m_Client = client;
        if (m_Client.DataProcessor != this)
        {
            m_Client.DataProcessor = this;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IClient<TMessage> m_Client;
}

// IClientDataProcessor<TMessage>
partial class ClientDataProcessor<TMessage> : IClientDataProcessor<TMessage>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public abstract void ProcessReceivedData([DisallowNull] TMessage data);

    /// <summary>
    /// Disconnects the <see cref="IClient{TMessage}"/> from the <see cref="IServer{TMessage}"/>.
    /// </summary>
    public void Disconnect() =>
        this.Client
            .Disconnect();

    /// <summary>
    /// Gets or sets the <see cref="IClient{TMessage}"/> associated with this processor.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    [NotNull]
    public IClient<TMessage> Client
    {
        get => m_Client;
        set
        {
            ExceptionHelpers.ThrowIfArgumentNull(value);

            m_Client = value;
            if (m_Client.DataProcessor != this)
            {
                m_Client.DataProcessor = this;
            }
        }
    }
}