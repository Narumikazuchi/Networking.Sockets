namespace Narumikazuchi.Networking.Sockets;

internal sealed partial class __ServerBuilder<TMessage>
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

    public IServerDataProcessor<TMessage>? DataProcessor
    {
        get;
        set;
    }

    public IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>>? Strategies
    {
        get;
        set;
    }

    public ServerAcceptCondition<TMessage>? AcceptCondition
    {
        get;
        set;
    }
}

// IServerUsingByteSerialization<T>
partial class __ServerBuilder<TMessage> : IServerUsingByteSerialization<TMessage>
{
    IServerUsingByteSerializationWithAcceptCondition<TMessage> IServerUsingByteSerialization<TMessage>.WithAcceptCondition(ServerAcceptCondition<TMessage> acceptCondition!!)
    {
        this.AcceptCondition = acceptCondition;
        return this;
    }

    IServerUsingByteSerializationWithDataProcessor<TMessage> IServerUsingByteSerialization<TMessage>.WithDataProcessor(IServerDataProcessor<TMessage> processor!!)
    {
        this.DataProcessor = processor;
        return this;
    }

    IServerUsingByteSerializationWithStrategies<TMessage> IServerUsingByteSerialization<TMessage>.WithSerializationStrategies(IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>> strategies!!)
    {
        this.Strategies = strategies;
        return this;
    }
}

// IServerUsingByteSerializationWithAcceptCondition<T>
partial class __ServerBuilder<TMessage> : IServerUsingByteSerializationWithAcceptCondition<TMessage>
{
    IServerUsingByteSerializationWithoutStrategies<TMessage> IServerUsingByteSerializationWithAcceptCondition<TMessage>.WithDataProcessor(IServerDataProcessor<TMessage> processor!!)
    {
        this.DataProcessor = processor;
        return this;
    }

    IServerUsingByteSerializationWithoutDataProcessor<TMessage> IServerUsingByteSerializationWithAcceptCondition<TMessage>.WithSerializationStrategies(IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>> strategies!!)
    {
        this.Strategies = strategies;
        return this;
    }
}

// IServerUsingByteSerializationWithDataProcessor<T>
partial class __ServerBuilder<TMessage> : IServerUsingByteSerializationWithDataProcessor<TMessage>
{
    IServerUsingByteSerializationWithoutStrategies<TMessage> IServerUsingByteSerializationWithDataProcessor<TMessage>.WithAcceptCondition(ServerAcceptCondition<TMessage> acceptCondition!!)
    {
        this.AcceptCondition = acceptCondition;
        return this;
    }

    IServerUsingByteSerializationWithoutAcceptCondition<TMessage> IServerUsingByteSerializationWithDataProcessor<TMessage>.WithSerializationStrategies(IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>> strategies!!)
    {
        this.Strategies = strategies;
        return this;
    }
}

// IServerUsingByteSerializationWithStrategies<T>
partial class __ServerBuilder<TMessage> : IServerUsingByteSerializationWithStrategies<TMessage>
{
    IServerUsingByteSerializationWithoutDataProcessor<TMessage> IServerUsingByteSerializationWithStrategies<TMessage>.WithAcceptCondition(ServerAcceptCondition<TMessage> acceptCondition!!)
    {
        this.AcceptCondition = acceptCondition;
        return this;
    }

    IServerUsingByteSerializationWithoutAcceptCondition<TMessage> IServerUsingByteSerializationWithStrategies<TMessage>.WithDataProcessor(IServerDataProcessor<TMessage> processor!!)
    {
        this.DataProcessor = processor;
        return this;
    }
}

// IServerUsingByteSerializationWithoutAcceptCondition<T>
partial class __ServerBuilder<TMessage> : IServerUsingByteSerializationWithoutAcceptCondition<TMessage>
{
    IServerUsingByteSerializationFinalizer<TMessage> IServerUsingByteSerializationWithoutAcceptCondition<TMessage>.WithAcceptCondition(ServerAcceptCondition<TMessage> acceptCondition!!)
    {
        this.AcceptCondition = acceptCondition;
        return this;
    }
}

// IServerUsingByteSerializationWithoutDataProcessor<T>
partial class __ServerBuilder<TMessage> : IServerUsingByteSerializationWithoutDataProcessor<TMessage>
{
    IServerUsingByteSerializationFinalizer<TMessage> IServerUsingByteSerializationWithoutDataProcessor<TMessage>.WithDataProcessor(IServerDataProcessor<TMessage> processor!!)
    {
        this.DataProcessor = processor;
        return this;
    }
}

// IServerUsingByteSerializationWithoutStrategies<T>
partial class __ServerBuilder<TMessage> : IServerUsingByteSerializationWithoutStrategies<TMessage>
{
    IServerUsingByteSerializationFinalizer<TMessage> IServerUsingByteSerializationWithoutStrategies<TMessage>.WithSerializationStrategies(IEnumerable<KeyValuePair<Type, ISerializationDeserializationStrategy<Byte[]>>> strategies!!)
    {
        this.Strategies = strategies;
        return this;
    }
}

// IServerUsingByteSerializationFinalizer<T>
partial class __ServerBuilder<TMessage> : IServerUsingByteSerializationFinalizer<TMessage>
{
    IServer<TMessage> IServerUsingByteSerializationFinalizer<TMessage>.Create() =>
        new __Server<TMessage>(this);
}