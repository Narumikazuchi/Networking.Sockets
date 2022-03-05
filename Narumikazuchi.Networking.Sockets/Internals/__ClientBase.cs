namespace Narumikazuchi.Networking.Sockets;

internal abstract partial class __ClientBase<TData>
{
    public Boolean IsConnected
    {
        get
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ClientBase<TData>));
            }
            return !m_Guid.Equals(default) &&
                   m_Socket.Connected;
        }
    }
}

// Non-Public
partial class __ClientBase<TData>
{
    protected __ClientBase(in Int32 port,
                         in Int32 bufferSize,
                         [AllowNull] IClientDataProcessor<TData>? processor)
    {
        if (port < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(port));
        }
        if (bufferSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        m_Socket = new Socket(addressFamily: AddressFamily.InterNetwork,
                              socketType: SocketType.Stream,
                              protocolType: ProtocolType.Tcp);
        m_DataBuffer = new Byte[bufferSize];
        this.Port = port;
        this.DataProcessor = processor;
    }

    [return: NotNull]
    protected abstract Byte[] SerializeToBytes([DisallowNull] TData data);

    [return: NotNull]
    protected abstract TData SerializeFromBytes([DisallowNull] Byte[] bytes);

    protected void InitiateSocket()
    {
        if (m_Disposed)
        {
            throw new ObjectDisposedException(objectName: nameof(__ClientBase<TData>));
        }

        m_Socket = new Socket(addressFamily: AddressFamily.InterNetwork,
                              socketType: SocketType.Stream,
                              protocolType: ProtocolType.Tcp);
    }

    protected void InitiateConnection()
    {
        if (m_Disposed)
        {
            throw new ObjectDisposedException(objectName: nameof(__ClientBase<TData>));
        }

        m_Socket.BeginReceive(buffer: m_DataBuffer,
                              offset: 0,
                              size: m_DataBuffer.Length,
                              socketFlags: SocketFlags.None,
                              callback: this.ReceiveCallback,
                              state: null);
    }

    protected void InitiateDisconnect(in Boolean raiseEvent)
    {
        if (m_Disposed)
        {
            throw new ObjectDisposedException(objectName: nameof(__ClientBase<TData>));
        }

        if (this.IsConnected)
        {
            m_Socket.Close();
            if (raiseEvent)
            {
                this.OnConnectionClosed();
            }
        }
    }

    protected void InitiateSend([DisallowNull] TData data!!)
    {
        if (m_Disposed)
        {
            throw new ObjectDisposedException(objectName: nameof(__ClientBase<TData>));
        }

        Byte[] bytes = this.SerializeToBytes(data);
        m_Socket.BeginSend(buffer: bytes,
                           offset: 0,
                           size: bytes.Length,
                           socketFlags: SocketFlags.None,
                           callback: this.SendCallback,
                           state: null);
        Thread.Sleep(1);
    }

    protected Socket Socket => 
        m_Socket;

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            Int32 received = m_Socket.EndReceive(asyncResult: result);
            Byte[]  data = new Byte[received];
            Array.Copy(sourceArray: m_DataBuffer,
                       sourceIndex: 0,
                       destinationArray: data,
                       destinationIndex: 0,
                       length: received);
            Array.Clear(array: m_DataBuffer,
                        index: 0,
                        length: received);

            this.ProcessIncomingData(data);

            m_Socket.BeginReceive(buffer: m_DataBuffer,
                                  offset: 0,
                                  size: m_DataBuffer.Length,
                                  socketFlags: SocketFlags.None,
                                  callback: this.ReceiveCallback,
                                  state: null);
        }
        catch (SocketException) { }
        catch (ObjectDisposedException) { }
    }

    private void SendCallback(IAsyncResult result)
    {
        try
        {
            m_Socket.EndSend(asyncResult: result);
            if (!this.IsConnected)
            {
                return;
            }
            m_Socket.BeginReceive(buffer: m_DataBuffer,
                                  offset: 0,
                                  size: m_DataBuffer.Length,
                                  socketFlags: SocketFlags.None,
                                  callback: this.ReceiveCallback,
                                  state: null);
        }
        catch (ObjectDisposedException) { }
    }

    private void ProcessIncomingData(Byte[] bytes!!)
    {
        if (bytes.Length == 64)
        {
            if (bytes.SequenceEqual(s_ShutdownSignature))
            {
                this.InitiateDisconnect(true);
                return;
            }
            if (bytes.Take(48)
                     .SequenceEqual(s_GuidSignature))
            {
                Byte[] guidBytes = bytes.Skip(48)
                                        .Take(16)
                                        .ToArray();
                m_Guid = new(b: guidBytes);
                this.OnConnectionEstablished();
                return;
            }
        }

        TData data = this.SerializeFromBytes(bytes);
        if (this.DataProcessor is null)
        {
            this.DataReceived?
                .Invoke(sender: this,
                        eventArgs: new(data: data));
            return;
        }
        this.DataProcessor
            .ProcessReceivedData(data: data);
    }

    private void OnConnectionEstablished() =>
        this.ConnectionEstablished?
            .Invoke(sender: this,
                    eventArgs: EventArgs.Empty);

    private void OnConnectionClosed() =>
        this.ConnectionClosed?
            .Invoke(sender: this,
                    eventArgs: EventArgs.Empty);

    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    private Socket m_Socket;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IClientDataProcessor<TData>? m_Processor;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Guid m_Guid = new();
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    private Byte[] m_DataBuffer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly Byte[] s_ShutdownSignature = new Byte[] { 0x72, 0x63, 0x53, 0x2E, 0x6D, 0x6E, 0x75, 0x74, 0x9C, 0x63, 0x5A, 0x78, 0x68, 0x2E, 0xBE, 0x67, 0xE4, 0x75, 0x69, 0x69, 0x6B, 0x65, 0x77, 0x74, 0x6F, 0x6B, 0xEE, 0x2E, 0x61, 0x4E, 0x77, 0x5, 0x61, 0xC2, 0x6B, 0x4E, 0x65, 0x73, 0xD1, 0x6F, 0x53, 0xF7, 0x7A, 0x86, 0x53, 0x68, 0x75, 0x74, 0x64, 0x6F, 0x77, 0x6E, 0x53, 0x65, 0x72, 0x65, 0x72, 0x43, 0x6C, 0x65, 0x68, 0x2E, 0xBE, 0x67 };
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly Byte[] s_GuidSignature = new Byte[] { 0x72, 0x63, 0x53, 0x2E, 0x6D, 0x6E, 0x75, 0x74, 0x9C, 0x63, 0x5A, 0x78, 0x68, 0x2E, 0xBE, 0x67, 0xE4, 0x75, 0x69, 0x69, 0x6B, 0x65, 0x77, 0x74, 0x6F, 0x6B, 0xEE, 0x2E, 0x61, 0x4E, 0x77, 0x5, 0x61, 0xC2, 0x6B, 0x4E, 0x65, 0x73, 0xD1, 0x6F, 0x53, 0xF7, 0x7A, 0x86, 0x47, 0x75, 0x69, 0x64 };
}

// IClient<TData>
partial class __ClientBase<TData> : IClient<TData>
{
    public abstract void Connect([DisallowNull] IPAddress address);

    public abstract void Disconnect();

    public abstract void Send([DisallowNull] TData data);

    public event EventHandler<IClient<TData>>? ConnectionEstablished;

    public event EventHandler<IClient<TData>>? ConnectionClosed;

    public event EventHandler<IClient<TData>, DataReceivedEventArgs<TData>>? DataReceived;

    public Int32 Port { get; }

    public Guid Guid => 
        m_Guid;

    public Int32 BufferSize
    {
        get

        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ClientBase<TData>));
            }
            return m_DataBuffer.Length;
        }
        set
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ClientBase<TData>));
            }
            m_DataBuffer = new Byte[value];
        }
    }

    [MaybeNull]
    public IClientDataProcessor<TData>? DataProcessor
    {
        get

        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ClientBase<TData>));
            }
            return m_Processor;
        }
        set
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ClientBase<TData>));
            }

            m_Processor = value;
            if (m_Processor is not null)
            {
                m_Processor.Client = this;
            }
        }
    }
}

// IDisposable
partial class __ClientBase<TData> : IDisposable
{
    public void Dispose()
    {
        if (m_Disposed)
        {
            return;
        }

        if (this.IsConnected)
        {
            this.InitiateDisconnect(false);
        }

        m_Disposed = true;
        GC.SuppressFinalize(this);
    }

    private Boolean m_Disposed = false;
}