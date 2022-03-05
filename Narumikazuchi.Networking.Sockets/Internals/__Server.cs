namespace Narumikazuchi.Networking.Sockets;

internal sealed partial class __Server
{
    public __Server(__ServerBuilder builder) : 
        base(port: builder.Port,
             bufferSize: builder.BufferSize,
             processor: builder.DataProcessor,
             condition: builder.AcceptCondition)
    { }

    public override void Start() =>
        this.InitiateStart();

    public override void Stop() =>
        this.InitiateStop();

    public override Boolean Disconnect(in Guid guid) =>
        this.InitiateDisconnect(guid);

    public override void Send(Byte[] data!!,
                              in Guid client) =>
        this.InitiateSend(data: data,
                          client: client);

    public override void Broadcast(Byte[] data!!) =>
        this.InitiateBroadcast(data);
}

// Non-Public
partial class __Server : __ServerBase<Byte[]>
{
    [return: NotNull]
    protected override Byte[] SerializeToBytes([DisallowNull] Byte[] data!!) =>
        data;

    [return: NotNull]
    protected override Byte[] SerializeFromBytes([DisallowNull] Byte[] bytes!!) =>
        bytes;
}