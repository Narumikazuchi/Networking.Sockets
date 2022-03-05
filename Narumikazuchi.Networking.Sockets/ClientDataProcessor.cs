namespace Narumikazuchi.Networking.Sockets;

/// <summary>
/// Provides the blueprint for data processing of an <see cref="IClient{TData}"/>.
/// </summary>
// Non-Public
public abstract partial class ClientDataProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientDataProcessor"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    protected ClientDataProcessor([DisallowNull] IClient<Byte[]> client!!)
    {
        m_Client = client;
        if (m_Client.DataProcessor != this)
        {
            m_Client.DataProcessor = this;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IClient<Byte[]> m_Client;
}

// IClientDataProcessor<Byte[]>
partial class ClientDataProcessor : IClientDataProcessor<Byte[]>
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public abstract void ProcessReceivedData([DisallowNull] Byte[] data);

    /// <summary>
    /// Disconnects the <see cref="Client"/> from the <see cref="IServer{TMessage}"/>.
    /// </summary>
    public void Disconnect() =>
        this.Client
            .Disconnect();

    /// <summary>
    /// Gets or sets the <see cref="Client"/> associated with this processor.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    [NotNull]
    public IClient<Byte[]> Client
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