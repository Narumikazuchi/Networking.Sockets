namespace Narumikazuchi.Networking.Sockets;

internal sealed partial class __ClientBuilder
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

    public IClientDataProcessor<Byte[]>? DataProcessor
    {
        get;
        set;
    }
}

// IClientWithBufferSize
partial class __ClientBuilder : IClientWithBufferSize
{
    IClientWithoutSerialization IClientWithBufferSize.WithPort(in Int32 port)
    {
        this.Port = port;
        return this;
    }
}

// IClientWithPort
partial class __ClientBuilder : IClientWithPort
{
    IClientWithoutSerialization IClientWithPort.WithBufferSize(in Int32 bufferSize)
    {
        this.BufferSize = bufferSize;
        return this;
    }
}

// IClientWithoutSerialization
partial class __ClientBuilder : IClientWithoutSerialization
{
    IClientUsingByteArrays IClientWithoutSerialization.UsingByteArrays() => 
        this;

    IClientUsingByteSerialization<TMessage> IClientWithoutSerialization.UsingByteSerialization<TMessage>()
    {
        __ClientBuilder<TMessage> builder = new()
        {
            Port = this.Port,
            BufferSize = this.BufferSize
        };
        return builder;
    }
}

// IClientUsingByteArrays
partial class __ClientBuilder : IClientUsingByteArrays
{
    IClientUsingByteArraysFinalizer IClientUsingByteArrays.WithDataProcessor(IClientDataProcessor<Byte[]> processor!!)
    {
        this.DataProcessor = processor;
        return this;
    }
}

// IClientUsingByteArraysFinalizer
partial class __ClientBuilder : IClientUsingByteArraysFinalizer
{
    IClient<Byte[]> IClientUsingByteArraysFinalizer.Create() =>
        new __Client(this);
}