namespace Narumikazuchi.Networking.Sockets;

internal abstract partial class __ServerBase<TData>
{
    public Boolean IsRunning =>
        m_IsRunning;
}

// Non-Public
partial class __ServerBase<TData>
{
    protected __ServerBase(in Int32 port,
                         in Int32 bufferSize,
                         [AllowNull] IServerDataProcessor<TData>? processor,
                         [AllowNull] ServerAcceptCondition<TData>? condition)
    {
        if (port < 0)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(port));
        }
        if (bufferSize < 0)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(bufferSize));
        }

        m_Socket = new(addressFamily: AddressFamily.InterNetwork,
                       socketType: SocketType.Stream,
                       protocolType: ProtocolType.Tcp);
        m_IsRunning = false;
        m_Condition = condition;
        m_DataBuffer = new Byte[bufferSize];
        this.Port = port;
        this.DataProcessor = processor;
    }

    [return: NotNull]
    protected abstract Byte[] SerializeToBytes([DisallowNull] TData data);

    [return: NotNull]
    protected abstract TData SerializeFromBytes([DisallowNull] Byte[] bytes);

    protected void InitiateStart()
    {
        if (m_Disposed)
        {
            throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
        }

        if (this.IsRunning)
        {
            return;
        }
        m_Socket = new(addressFamily: AddressFamily.InterNetwork,
                       socketType: SocketType.Stream,
                       protocolType: ProtocolType.Tcp);

        try
        {
            m_Socket.Bind(localEP: new IPEndPoint(address: IPAddress.Any,
                                                  port: this.Port));
            m_Socket.Listen(backlog: 12);
            m_Clients.Clear();
            m_Socket.BeginAccept(callback: this.AcceptCallback,
                                 state: null);
            m_IsRunning = true;
        }
        catch
        {
            m_IsRunning = false;
        }
    }

    protected void InitiateStop()
    {
        if (m_Disposed)
        {
            throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
        }

        if (this.Clients
                .Count > 0)
        {
            foreach (KeyValuePair<Guid, Socket> kv in m_Clients)
            {
                if (kv.Value
                      .Connected)
                {
                    try
                    {
                        this.InitiateSend(bytes: s_ShutdownSignature,
                                          client: kv.Value);
                    }
                    catch (SocketException) { }
                    kv.Value
                      .Close();
                    this.OnClientDisconnected(client: kv.Key,
                                              connection: ConnectionType.ConnectionClosed);
                }
            }
        }
        m_Clients.Clear();
        m_Socket.Close();
        m_Socket.Dispose();
        m_IsRunning = false;
    }

    protected Boolean InitiateDisconnect(in Guid guid)
    {
        if (m_Disposed)
        {
            throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
        }

        if (!m_Clients.ContainsKey(guid))
        {
            return false;
        }

        if (!m_Clients[guid].Connected)
        {
            m_Clients.Remove(guid);
            this.OnClientDisconnected(client: guid,
                                      connection: ConnectionType.ConnectionLost);
            return true;
        }

        this.InitiateSend(bytes: s_ShutdownSignature,
                          client: m_Clients[guid]);
        m_Clients[guid].Close();
        m_Clients.Remove(guid);
        this.OnClientDisconnected(client: guid,
                                  connection: ConnectionType.ConnectionLost);
        return true;
    }

    protected void InitiateSend([DisallowNull] TData data!!,
                                Guid client)
    {
        if (m_Disposed)
        {
            throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
        }
        if (!m_Clients.ContainsKey(client))
        {
            throw new KeyNotFoundException();
        }

        Socket socket = m_Clients[client];
        Byte[] bytes = this.SerializeToBytes(data);
        this.InitiateSend(bytes: bytes,
                          client: socket);
    }

    protected void InitiateBroadcast([DisallowNull] TData data!!)
    {
        if (m_Disposed)
        {
            throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
        }

        Byte[] bytes = this.SerializeToBytes(data);
        this.InitiateBroadcast(bytes: bytes);
    }

    protected Socket Socket => 
        m_Socket;

    private void InitiateSend(Byte[] bytes!!,
                              Socket client!!)
    {
        if (!client.Connected)
        {
            throw new NotConnectedException(socket: client);
        }

        client.BeginSend(buffer: bytes,
                         offset: 0,
                         size: bytes.Length,
                         socketFlags: SocketFlags.None,
                         callback: this.SendCallback,
                         state: client);
    }

    private void InitiateBroadcast(Byte[] bytes!!)
    {
        foreach (Socket client in m_Clients.Values)
        {
            if (!client.Connected)
            {
                continue;
            }
            client.BeginSend(buffer: bytes,
                             offset: 0,
                             size: bytes.Length,
                             socketFlags: SocketFlags.None,
                             callback: this.SendCallback,
                             state: client);
            Thread.Sleep(millisecondsTimeout: 1);
        }
    }

    private void AcceptCallback(IAsyncResult result)
    {
        try
        {
            Socket socket = m_Socket.EndAccept(asyncResult: result);
            Guid guid = Guid.NewGuid();

            if (m_Condition is not null &&
                !m_Condition.Invoke(server: this,
                                    guidOfNewClient: guid))
            {
                socket.Close();
                return;
            }

            m_Clients.Add(key: guid, 
                          value: socket);
            socket.BeginReceive(buffer: m_DataBuffer,
                                offset: 0,
                                size: m_DataBuffer.Length,
                                socketFlags: SocketFlags.None,
                                callback: this.ReceiveCallback,
                                state: socket);
            this.InitiateSend(bytes: s_GuidSignature.Concat(guid.ToByteArray())
                                                    .ToArray(),
                              client: socket);

            this.OnClientConnected(client: guid,
                                   connection: ConnectionType.ConnectionEstablished);

            if (this.IsRunning)
            {
                m_Socket.BeginAccept(callback: this.AcceptCallback,
                                     state: null);
            }
        }
        catch (ObjectDisposedException) { }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            Socket socket = (Socket)result.AsyncState!;
            Int32 received = socket.EndReceive(asyncResult: result);
            Byte[] data = new Byte[received];
            Array.Copy(sourceArray: m_DataBuffer,
                       sourceIndex: 0,
                       destinationArray: data,
                       destinationIndex: 0,
                       length: received);
            Array.Clear(array: m_DataBuffer,
                        index: 0,
                        length: received);

            this.ProcessIncomingData(bytes: data,
                                     client: socket);
            if (this.IsRunning)
            {
                socket.BeginReceive(buffer: m_DataBuffer,
                                    offset: 0,
                                    size: m_DataBuffer.Length,
                                    socketFlags: SocketFlags.None,
                                    callback: this.ReceiveCallback,
                                    state: socket);
            }
        }
        catch (SocketException) { }
        catch (ObjectDisposedException) { }
    }

    private void SendCallback(IAsyncResult result)
    {
        try
        {
            Socket socket = (Socket)result.AsyncState!;
            socket.EndSend(asyncResult: result);
            if (this.IsRunning)
            {
                socket.BeginReceive(buffer: m_DataBuffer,
                                    offset: 0,
                                    size: m_DataBuffer.Length,
                                    socketFlags: SocketFlags.None,
                                    callback: this.ReceiveCallback,
                                    state: socket);
            }
        }
        catch (ObjectDisposedException) { }
    }

    private void ProcessIncomingData(Byte[] bytes!!,
                                     Socket? client)
    {
        if (client is null)
        {
            return;
        }

        Guid guid;
        if (bytes.Length == 64)
        {
            if (bytes.SequenceEqual(s_ShutdownSignature))
            {
                if (client.Connected)
                {
                    client.Close();
                }
                if (!m_Clients.ContainsValue(client))
                {
                    return;
                }
                guid = m_Clients.First(kv => kv.Value == client)
                                .Key;
                m_Clients.Remove(guid);
                this.OnClientDisconnected(client: guid,
                                          connection: ConnectionType.ConnectionLost);
                return;
            }
        }

        if (!m_Clients.ContainsValue(client))
        {
            return;
        }
        guid = m_Clients.First(kv => kv.Value == client)
                        .Key;
        TData data = this.SerializeFromBytes(bytes);
        if (this.DataProcessor is null)
        {
            this.DataReceived?
                .Invoke(sender: this,
                        eventArgs: new(data: data,
                                       fromClient: guid));
            return;
        }
        this.DataProcessor
            .ProcessReceivedData(data: data,
                                 fromClient: guid);
    }

    private void OnClientConnected(Guid client,
                                   ConnectionType connection) =>
        this.ClientConnected?
            .Invoke(sender: this,
                    eventArgs: new(whichClient: client,
                                   type: connection));

    private void OnClientDisconnected(Guid client,
                                      ConnectionType connection) =>
        this.ClientDisconnected?
            .Invoke(sender: this,
                    eventArgs: new(whichClient: client,
                                   type: connection));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<Guid, Socket> m_Clients = new();
    private Socket m_Socket;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IServerDataProcessor<TData>? m_Processor;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ServerAcceptCondition<TData>? m_Condition;
    private Byte[] m_DataBuffer;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Boolean m_IsRunning;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly Byte[] s_ShutdownSignature = new Byte[] { 0x72, 0x63, 0x53, 0x2E, 0x6D, 0x6E, 0x75, 0x74, 0x9C, 0x63, 0x5A, 0x78, 0x68, 0x2E, 0xBE, 0x67, 0xE4, 0x75, 0x69, 0x69, 0x6B, 0x65, 0x77, 0x74, 0x6F, 0x6B, 0xEE, 0x2E, 0x61, 0x4E, 0x77, 0x5, 0x61, 0xC2, 0x6B, 0x4E, 0x65, 0x73, 0xD1, 0x6F, 0x53, 0xF7, 0x7A, 0x86, 0x53, 0x68, 0x75, 0x74, 0x64, 0x6F, 0x77, 0x6E, 0x53, 0x65, 0x72, 0x65, 0x72, 0x43, 0x6C, 0x65, 0x68, 0x2E, 0xBE, 0x67 };
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly Byte[] s_GuidSignature = new Byte[] { 0x72, 0x63, 0x53, 0x2E, 0x6D, 0x6E, 0x75, 0x74, 0x9C, 0x63, 0x5A, 0x78, 0x68, 0x2E, 0xBE, 0x67, 0xE4, 0x75, 0x69, 0x69, 0x6B, 0x65, 0x77, 0x74, 0x6F, 0x6B, 0xEE, 0x2E, 0x61, 0x4E, 0x77, 0x5, 0x61, 0xC2, 0x6B, 0x4E, 0x65, 0x73, 0xD1, 0x6F, 0x53, 0xF7, 0x7A, 0x86, 0x47, 0x75, 0x69, 0x64 };
}

// IServer<TData>
partial class __ServerBase<TData> : IServer<TData>
{
    public abstract void Start();

    public abstract void Stop();

    public abstract Boolean Disconnect(in Guid guid);

    public abstract void Send([DisallowNull] TData data,
                              in Guid client);

    public abstract void Broadcast([DisallowNull] TData data);

    public event EventHandler<IServer<TData>, ConnectionEventArgs>? ClientConnected;

    public event EventHandler<IServer<TData>, ConnectionEventArgs>? ClientDisconnected;

    public event EventHandler<IServer<TData>, DataReceivedEventArgs<TData>>? DataReceived;

    public Int32 Port { get; }

    public Int32 BufferSize
    {
        get
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
            }
            return m_DataBuffer.Length;
        }
        set
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
            }
            m_DataBuffer = new Byte[value];
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    [NotNull]
    public IReadOnlyList<Guid> Clients
    {
        get
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
            }
            return m_Clients.Keys
                            .ToList();
        }
    }

    [MaybeNull]
    public IServerDataProcessor<TData>? DataProcessor
    {
        get
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
            }
            return m_Processor;
        }
        set
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
            }

            m_Processor = value;
            if (m_Processor is not null)
            {
                m_Processor.Server = this;
            }
        }
    }

    [MaybeNull]
    public ServerAcceptCondition<TData>? AcceptCondition
    {
        get
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
            }
            return m_Condition;
        }
        set
        {
            if (m_Disposed)
            {
                throw new ObjectDisposedException(objectName: nameof(__ServerBase<TData>));
            }

            if (value is null)
            {
                throw new ArgumentNullException(paramName: nameof(value));
            }
            m_Condition = value;
        }
    }
}

// IDisposable
partial class __ServerBase<TData> : IDisposable
{
    public void Dispose()
    {
        if (m_Disposed)
        {
            return;
        }

        if (m_IsRunning)
        {
            this.Stop();
        }

        m_Disposed = true;
        GC.SuppressFinalize(this);
    }

    private Boolean m_Disposed = false;
}