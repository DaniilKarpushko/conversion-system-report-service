using Confluent.Kafka;
using Google.Protobuf;
using KafkaInfrastructure.Exceptions;

namespace KafkaInfrastructure.Serializers;

public class ProtobufKafkaSerializer<T> : ISerializer<T>, IDeserializer<T> where T : IMessage<T>, new()
{
    private static readonly MessageParser<T> Parser = new MessageParser<T>(() => new T());
    
    public byte[] Serialize(T data, SerializationContext context)
    {
        return data.ToByteArray();
    }

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return isNull == false ? Parser.ParseFrom(data) : throw new KafkaMessageParsingExceptions("Message is null");
    }
}