namespace Narumikazuchi.Networking.Sockets;

internal sealed partial class __Client
{
    public __Client(__ClientBuilder builder!!) : 
        base(port: builder.Port,
             bufferSize: builder.BufferSize,
             processor: builder.DataProcessor)
    { }

    public override void Connect(IPAddress address!!)
    {
        if (this.IsConnected)
        {
            return;
        }

        this.InitiateSocket();
        m_CurrentAttempts = 0;
        this.LoopConnect(address);
    }

    public override void Disconnect() =>
        this.InitiateDisconnect(true);

    public override void Send(Byte[] data!!)
    {
        if (!this.IsConnected)
        {
            throw new NotConnectedException(socket: this.Socket);
        }

        this.InitiateSend(data);
    }
}

// Non-Public
partial class __Client : __ClientBase<Byte[]>
{
    [return: NotNull]
    protected override Byte[] SerializeToBytes(Byte[] data!!) => 
        data;

    [return: NotNull]
    protected override Byte[] SerializeFromBytes(Byte[] bytes!!) => 
        bytes;

    private void LoopConnect(IPAddress address!!)
    {
        while (!this.Socket
                    .Connected &&
               m_CurrentAttempts < MAXATTEMPTS)
        {
            try
            {
                m_CurrentAttempts++;
                this.Socket
                    .Connect(address: address,
                             port: this.Port);
            }
            catch (SocketException) { }
        }
        if (m_CurrentAttempts == MAXATTEMPTS &&
            !this.Socket
                 .Connected)
        {
            throw new MaximumAttemptsExceededException();
        }
        if (this.Socket
                .Connected)
        {
            this.InitiateConnection();
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    private Int32 m_CurrentAttempts = 0;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const Int32 MAXATTEMPTS = 20;
}