namespace Narumikazuchi.Networking.Sockets;

internal sealed partial class __ClientBuilder<TMessage>
    where TMessage : IDeserializable<TMessage>, ISerializable
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

    public IClientDataProcessor<TMessage>? DataProcessor
    {
        get;
        set;
    }

    public IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>>? Strategies
    {
        get;
        set;
    }
}

// IClientUsingByteSerialization<T>
partial class __ClientBuilder<TMessage> : IClientUsingByteSerialization<TMessage>
{
    IClientUsingByteSerializationWithoutStrategies<TMessage> IClientUsingByteSerialization<TMessage>.WithDataProcessor(IClientDataProcessor<TMessage> processor!!)
    {
        this.DataProcessor = processor;
        return this;
    }

    IClientUsingByteSerializationWithoutProcessor<TMessage> IClientUsingByteSerialization<TMessage>.WithSerializationStrategies(IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>> strategies!!)
    {
        this.Strategies = strategies;
        return this;
    }
}

// IClientUsingByteSerializationFinalizer<T>
partial class __ClientBuilder<TMessage> : IClientUsingByteSerializationFinalizer<TMessage>
{
    IClient<TMessage> IClientUsingByteSerializationFinalizer<TMessage>.Create() =>
        new __Client<TMessage>(this);
}

// IClientUsingByteSerializationWithoutStrategies<T>
partial class __ClientBuilder<TMessage> : IClientUsingByteSerializationWithoutStrategies<TMessage>
{
    IClientUsingByteSerializationFinalizer<TMessage> IClientUsingByteSerializationWithoutStrategies<TMessage>.WithSerializationStrategies(IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>> strategies!!)
    {
        this.Strategies = strategies;
        return this;
    }
}

// IClientUsingByteSerializationWithoutProcessor<T>
partial class __ClientBuilder<TMessage> : IClientUsingByteSerializationWithoutProcessor<TMessage>
{
    IClientUsingByteSerializationFinalizer<TMessage> IClientUsingByteSerializationWithoutProcessor<TMessage>.WithDataProcessor(IClientDataProcessor<TMessage> processor!!)
    {
        this.DataProcessor = processor;
        return this;
    }
}