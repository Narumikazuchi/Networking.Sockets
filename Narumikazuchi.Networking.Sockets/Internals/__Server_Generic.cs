namespace Narumikazuchi.Networking.Sockets;

internal sealed partial class __Server<TMessage>
    where TMessage : IDeserializable<TMessage>, ISerializable
{
    public __Server(__ServerBuilder<TMessage> builder) : 
        base(port: builder.Port,
             bufferSize: builder.BufferSize,
             processor: builder.DataProcessor,
             condition: builder.AcceptCondition)
    {
        m_Serializer = CreateByteSerializer
                      .ConfigureForOwnedType<TMessage>()
                      .ForSerializationAndDeserialization()
                      .UseDefaultStrategies()
                      .UseStrategies(builder.Strategies ?? Array.Empty<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>>())
                      .Create();
    }

    public override void Start() =>
        this.InitiateStart();

    public override void Stop() =>
        this.InitiateStop();

    public override Boolean Disconnect(in Guid guid) =>
        this.InitiateDisconnect(guid);

    public override void Send(TMessage data!!,
                              in Guid client) =>
        this.InitiateSend(data: data,
                          client: client);

    public override void Broadcast(TMessage data!!) =>
        this.InitiateBroadcast(data);
}

// Non-Public
partial class __Server<TMessage> : __ServerBase<TMessage>
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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly IByteSerializerDeserializer<TMessage> m_Serializer;
}