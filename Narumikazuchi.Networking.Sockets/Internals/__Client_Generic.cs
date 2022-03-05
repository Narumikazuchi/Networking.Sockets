namespace Narumikazuchi.Networking.Sockets;

internal sealed partial class __Client<TMessage>
    where TMessage : IDeserializable<TMessage>, ISerializable
{
    public __Client(__ClientBuilder<TMessage> builder!!) : 
        base(port: builder.Port,
             bufferSize: builder.BufferSize,
             processor: builder.DataProcessor)
    {
        m_Serializer = CreateByteSerializer
                      .ConfigureForOwnedType<TMessage>()
                      .ForSerializationAndDeserialization()
                      .UseDefaultStrategies()
                      .UseStrategies(builder.Strategies ?? Array.Empty<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>>())
                      .Create();
    }

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

    public override void Send(TMessage data!!)
    {
        if (!this.IsConnected)
        {
            throw new NotConnectedException(socket: this.Socket);
        }

        this.InitiateSend(data);
    }
}

// Non-Public
partial class __Client<TMessage> : __ClientBase<TMessage>
{
    [return: NotNull]
    protected override Byte[] SerializeToBytes(TMessage data!!)
    {
        using MemoryStream stream = new();
        m_Serializer.Serialize(stream: stream,
                               graph: data);
        return stream.ToArray();
    }

    [return: NotNull]
    protected override TMessage SerializeFromBytes(Byte[] bytes!!)
    {
        using MemoryStream stream = new(buffer: bytes);
        stream.Position = 0;
        return m_Serializer.Deserialize(stream: stream)!;
    }

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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly IByteSerializerDeserializer<TMessage> m_Serializer;
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    private Int32 m_CurrentAttempts = 0;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const Int32 MAXATTEMPTS = 20;
}