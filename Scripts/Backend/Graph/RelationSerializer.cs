using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using RelationSpace;
using TypeRegistrySpace;
using Optional;
using Optional.Unsafe;
namespace RelationSerializerSpace;
///<summary> A class which stores info about relation. Designed to be trivial to export to JSON, or any other format. The str is of form "{elemStrRepr:TypeSerializer,...}"</summary>
public record IntermediateFormatToSerializeTo(String DomainSetStr, String CodomanSetStr, String RelationSetStr)
{
}
public class RelationSerializer
{
    private object DeserializePairStr(String givenPair)
    {
        //We take the last colon, knowing that the AssemblyQualifiedName of a type can never have a colon
        var lastColonIndex = givenPair.LastIndexOf(':');
        var elemStr = givenPair[..lastColonIndex];
        var typeSerializerStr = givenPair[(lastColonIndex)..];
        var typeSerializerT = TypeSerializationRegistry.GetTypeSerializerFromRegisteredStr(typeSerializerStr).ValueOrFailure("Given value's type is not registered");
        var typeSerializer = Activator.CreateInstance(typeSerializerT);
        var elem = typeSerializerT.GetMethod("DeserializeFromString").Invoke(typeSerializer, new object[] { elemStr });
        return elem;
    }
    private String SerializeHashSetToString<T>(ImmutableHashSet<T> givenHashSet, ITypeSerializer<T> serializer)
    {
        if (givenHashSet.Count == 0)
        {
            return "{}";
        }
        var builder = new StringBuilder("{");
        foreach (var elem in givenHashSet)
        {
            builder.Append($"{serializer.SerializeFrom(elem)}:{TypeSerializationRegistry.GetTypeStrForRegisteredTypeSerializer<T>()},");
        }
        builder.Remove(builder.Length - 1, 1).Append("}");
        return builder.ToString();
    }
    ///<summary> The string must be of form "{elemStrRepr:TypeSerializer,...} Returns a ImmutableHashSet<TypeSerializer> containing the elements. Make sure to cast it before use"</summary>
    private object DeserializeHashSetFromString(String str)
    {
        str = str.Trim();
        if (str.First() != '{' || str.Last() != '}')
        {
            throw new ArgumentException("Domain set must be of form {...}");
        }
        if (str == "{}")
        {
            throw new ArgumentException("Set is empty, so no type information can be deduced");
        }
        var strWithoutBraces = str[1..^1];
    }
    public Option<IntermediateFormatToSerializeTo> SerializeToString<DomainType, CodomainType>(Relation<DomainType, CodomainType> givenRelation)
    {
        var domainSetSerializer = TypeSerializationRegistry.GetTypeSerializerForType<DomainType>();
        var codomainSetSerializer = TypeSerializationRegistry.GetTypeSerializerForType<CodomainType>();
        var domainSetStr = domainSetSerializer.Map((serializer) => SerializeHashSetToString(givenRelation.GetDomainSet(), serializer));
        var codomainSetStr = codomainSetSerializer.Map((serializer) => SerializeHashSetToString(givenRelation.GetCodomainSet(), serializer));
        if (givenRelation.Count() == 0)
        {
            return Option.Some(new IntermediateFormatToSerializeTo(domainSetStr.ValueOrFailure("Failed to serialize domain set"), codomainSetStr.ValueOrFailure("Failed to serialize codomain set"), "{}"));

        }
        var setForm = givenRelation.ConvertToNormalForm();
        var relationBuilder = new StringBuilder("{");
        foreach (var pair in setForm)
        {
            var (first, second) = pair;
            var firstSerializer = TypeSerializationRegistry.GetTypeSerializerFromRegisteredStr(first.GetType().AssemblyQualifiedName).Map((serializerT) => Activator.CreateInstance(serializerT));
            var secondSerializer = TypeSerializationRegistry.GetTypeSerializerFromRegisteredStr(second.GetType().AssemblyQualifiedName).Map((serializerT) => Activator.CreateInstance(serializerT));
            if (!firstSerializer.HasValue || !secondSerializer.HasValue)
            {
                throw new ArgumentException($"Invalid type of domain or codomain, or types of domain and codomain are not registered.");
            }
            var firstStrOption = firstSerializer.Map((serializer) => ((ITypeSerializer<DomainType>)serializer).SerializeFrom(first));
            var secondStrOption = secondSerializer.Map((serializer) => ((ITypeSerializer<CodomainType>)serializer).SerializeFrom(second));
            var firstStr = firstStrOption.ValueOrFailure("Failed to serialize first element");
            var secondStr = secondStrOption.ValueOrFailure("Failed to serialize second element");
            relationBuilder.Append($"{firstStr}:{secondStr},");
        }
        relationBuilder.Remove(relationBuilder.Length - 1, 1);
        relationBuilder.Append("}");
        return Option.Some<IntermediateFormatToSerializeTo>(new IntermediateFormatToSerializeTo(domainSetStr.ValueOrFailure("Failed to serialize domain set"), codomainSetStr.ValueOrFailure("Failed to serialize codomain set"), relationBuilder.ToString()));
    }
    public Option<object> DeserializeFromString(IntermediateFormatToSerializeTo serialized)
    {
        var domain = serialized.DomainSetStr;
        var codomain = serialized.CodomanSetStr;
        if (domain.First() != '{' || codomain.First() != '{' || domain.Last() != '}' || codomain.Last() != '}')
        {
            return Option.None<object>();
        }
        var domainElements = domain[1..^1];
        var codomainElements = codomain[1..^1];
        var domainSetDeserialized = domainElements.Split(',').Select((elem) => elem.Trim());
        var codomainSetDeserialized = codomainElements.Split(',').Select((elem) => elem.Trim());
    }
}
