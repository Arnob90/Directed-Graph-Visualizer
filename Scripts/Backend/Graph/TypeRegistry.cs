using Godot;
using System;
using System.Collections.Generic;
using Optional;
namespace TypeRegistrySpace;
public interface ITypeSerializer<T>
{
    public T DeserializeFromString(string givenString);
    public String SerializeFrom(T givenElem);
}
public class IntSerializer : ITypeSerializer<int>
{
    public int DeserializeFromString(string givenString)
    {
        return int.Parse(givenString);
    }
    public string SerializeFrom(int givenElem)
    {
        return givenElem.ToString();
    }
}
public class FloatSerializer : ITypeSerializer<float>
{
    public float DeserializeFromString(string givenString)
    {
        return float.Parse(givenString);
    }
    public string SerializeFrom(float givenElem)
    {
        return givenElem.ToString();
    }
}
public class StringSerializer : ITypeSerializer<string>
{
    public string DeserializeFromString(string givenString)
    {
        return givenString;
    }
    public string SerializeFrom(string givenElem)
    {
        return givenElem;
    }
}
public class ByteSerialize : ITypeSerializer<byte>
{
    public byte DeserializeFromString(string givenString)
    {
        return byte.Parse(givenString);
    }
    public string SerializeFrom(byte givenElem)
    {
        return givenElem.ToString();
    }
}
public partial class TypeSerializationRegistry
{
    private static Dictionary<String, Type> TypeSerializerLookup = new Dictionary<string, Type>();
    private static Dictionary<Type, String> InverseTypeSerializerLookup = new Dictionary<Type, String>();
    static TypeSerializationRegistry()
    {
        (new TypeSerializationRegistry()).Register<int>(new IntSerializer()).Register<String>(new StringSerializer()).Register<float>(new FloatSerializer());
    }
    public TypeSerializationRegistry Register<T>(ITypeSerializer<T> givenSerializer)
    {
        TypeSerializerLookup.Add(typeof(T).AssemblyQualifiedName, givenSerializer.GetType());
        InverseTypeSerializerLookup.Add(givenSerializer.GetType(), typeof(T).AssemblyQualifiedName);
        return this;
    }
    public static Option<String> GetTypeStrForRegisteredTypeSerializer(Type givenT)
    {
        return InverseTypeSerializerLookup.ContainsKey(givenT) ? Option.Some(InverseTypeSerializerLookup[givenT]) : Option.None<String>();
    }
    public static Option<String> GetTypeStrForRegisteredTypeSerializer<T>()
    {
        return GetTypeStrForRegisteredTypeSerializer(typeof(T));
    }
    public static Option<Type> GetTypeSerializerFromRegisteredStr(String givenT)
    {
        return TypeSerializerLookup.ContainsKey(givenT) ? Option.Some(TypeSerializerLookup[givenT]) : Option.None<Type>();
    }
    public static Option<Type> GetTypeSerializerForType(Type givenT)
    {
        return TypeSerializerLookup.ContainsKey(givenT.AssemblyQualifiedName) ? Option.Some(TypeSerializerLookup[givenT.AssemblyQualifiedName]) : Option.None<Type>();
    }
    public static Option<ITypeSerializer<T>> GetTypeSerializerForType<T>()
    {
        return GetTypeSerializerForType(typeof(T)).Map((type) => (ITypeSerializer<T>)Activator.CreateInstance(type));
    }
}
