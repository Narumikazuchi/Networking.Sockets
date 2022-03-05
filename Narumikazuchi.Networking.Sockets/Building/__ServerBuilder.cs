namespace Narumikazuchi.Networking.Sockets;

internal sealed partial class __ServerBuilder
{
    public Int32 Port
    {
        get;
        set;
    }

    public Int32 BufferSize
    {
        get;
        set;
    }

    public IServerDataProcessor<Byte[]>? DataProcessor
    {
        get;
        set;
    }

    public ServerAcceptCondition<Byte[]>? AcceptCondition
    {
        get;
        set;
    }
}

// IClientWithBufferSize
partial class __ServerBuilder : IServerWithBufferSize
{
    IServerWithoutSerialization IServerWithBufferSize.WithPort(in Int32 port)
    {
        this.Port = port;
        return this;
    }
}

// IClientWithPort
partial class __ServerBuilder : IServerWithPort
{
    IServerWithoutSerialization IServerWithPort.WithBufferSize(in Int32 bufferSize)
    {
        this.BufferSize = bufferSize;
        return this;
    }
}

// IClientWithoutSerialization
partial class __ServerBuilder : IServerWithoutSerialization
{
    IServerUsingByteArrays IServerWithoutSerialization.UsingByteArrays() => 
        this;

    IServerUsingByteSerialization<TMessage> IServerWithoutSerialization.UsingByteSerialization<TMessage>()
    {
        __ServerBuilder<TMessage> builder = new()
        {
            Port = this.Port,
            BufferSize = this.BufferSize
        };
        return builder;
    }
}

// IClientUsingByteArrays
partial class __ServerBuilder : IServerUsingByteArrays
{
    IServerUsingByteArraysFinalizer IServerUsingByteArrays.WithDataProcessor(IServerDataProcessor<Byte[]> processor!!)
    {
        this.DataProcessor = processor;
        return this;
    }
}

// IClientUsingByteArraysFinalizer
partial class __ServerBuilder : IServerUsingByteArraysFinalizer
{
    IServer<Byte[]> IServerUsingByteArraysFinalizer.Create() =>
        new __Server(this);
}