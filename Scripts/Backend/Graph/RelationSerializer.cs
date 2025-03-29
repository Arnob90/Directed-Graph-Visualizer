using System;
using Godot;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using RelationSpace;
using TypeRegistrySpace;
using Optional;
using Optional.Unsafe;
using MiscSpace;
namespace RelationSerializerSpace;
///<summary> A class which stores info about relation. Designed to be trivial to export to JSON, or any other format. The str is of form "{elemStrRepr:TypeSerializer,...}"</summary>
public record IntermediateFormatToSerializeTo(HashSetSerialized DomainSet,HashSetSerialized CodomainSet,String[] RelationMapStr);
public record HashSetSerialized(String[] SetStr,String TypeStr);
public class RelationSerializer
{
    private Option<HashSetSerialized> SerializeHashSetToStr<T>(ImmutableHashSet<T> givenSet)
    {
        var typeSerializer=TypeSerializationRegistry.GetTypeSerializerForType<T>();
        typeSerializer.Match(
                some:(_)=>{},
                none:()=>GD.PrintErr("Type is not registered")
        );
        var serializedSetStr=typeSerializer.Map((serializer)=>
        {
            List<String> requiredStrRepr=new();
            foreach(var elem in givenSet)
            {
                requiredStrRepr.Add(serializer.SerializeFrom(elem));
            }
            return requiredStrRepr;
        });
        var serializedSet=serializedSetStr.Map((setRepr)=>new HashSetSerialized(setRepr.ToArray(),TypeSerializationRegistry.GetStringReprForType<T>()));
        return serializedSet;
    }
    private ImmutableHashSet<T> DeserializeHashSetFromStr<T>(HashSetSerialized givenSet,ITypeSerializer<T> typeSerializer)
    {
        var requiredSet=ImmutableHashSet<T>.Empty;
        foreach(var elem in givenSet.SetStr)
        {
            requiredSet=requiredSet.Add(typeSerializer.DeserializeFromString(elem));
        }
        return requiredSet;
    }
    private Option<object> DeserializeHashSetFromStr(HashSetSerialized givenSet)
    {
        var typeSerializerT=TypeSerializationRegistry.GetTypeSerializerFromRegisteredStr(givenSet.TypeStr);
        var typeSerializer=typeSerializerT.Map((typeSerializerT)=>Activator.CreateInstance(typeSerializerT));
        var typeToBeSerialized=typeSerializerT.Map((typeSerializerT)=>typeSerializerT.GetInterface(typeof(ITypeSerializer<>).Name).GetGenericArguments()[0]);
        if(!(typeToBeSerialized.HasValue && typeSerializer.HasValue))
        {
            return Option.None<object>();
        }
        var typeToBeSerializedKnown=typeToBeSerialized.ValueOrFailure();
        var typeSerializerKnown=typeSerializer.ValueOrFailure();
        var otherMethodToCall=this.GetType().GetMethods().Where((m)=>m.Name=="DeserializeHashSetFromStr" && m.IsGenericMethod && m.GetParameters().Length==2).First();
        return Option.Some<object>(otherMethodToCall.MakeGenericMethod(typeToBeSerializedKnown).Invoke(this,new object[]{givenSet,typeSerializerKnown}));
    }
    public Option<IntermediateFormatToSerializeTo> SerializeRelationToIntermediateFormat<DomainType,CodomainType>(Relation<DomainType,CodomainType> relation)
    {
        var domainSerialized=SerializeHashSetToStr<DomainType>(relation.GetDomainSet());
        var codomainSerialized=SerializeHashSetToStr<CodomainType>(relation.GetCodomainSet());
        if(!domainSerialized.HasValue || !codomainSerialized.HasValue)
        {
            return Option.None<IntermediateFormatToSerializeTo>();
        }
        var domainSerializedKnown=domainSerialized.ValueOrFailure();
        var codomainSerializedKnown=codomainSerialized.ValueOrFailure();
        if(relation.RelationMap.Count==0)
        {
            return Option.Some<IntermediateFormatToSerializeTo>(new IntermediateFormatToSerializeTo(domainSerializedKnown,codomainSerializedKnown,new String[]{}));
        }
        var domainTypeSerializer=TypeSerializationRegistry.GetTypeSerializerForType<DomainType>();
        var codomainTypeSerializer=TypeSerializationRegistry.GetTypeSerializerForType<CodomainType>();
        var finalRelation=new List<String>();
        foreach(var (domainElem,codomainElem) in relation.ConvertToNormalForm())
        {
            var domainElemSerialized=domainTypeSerializer.Map((serializer)=>serializer.SerializeFrom(domainElem));
            var codomainElemSerialized=codomainTypeSerializer.Map((serializer)=>serializer.SerializeFrom(codomainElem));
            if(!(domainElemSerialized.HasValue && codomainElemSerialized.HasValue))
            {
                throw new ArgumentException("Type is not registered, or serialization failed");
            }
            var domainElemSerializedKnown=domainElemSerialized.ValueOrFailure();
            var codomainElemSerializedKnown=codomainElemSerialized.ValueOrFailure();
            finalRelation.Add($"({domainElemSerializedKnown},{codomainElemSerializedKnown})");
        }
        return Option.Some<IntermediateFormatToSerializeTo>(new IntermediateFormatToSerializeTo(domainSerializedKnown,codomainSerializedKnown,finalRelation.ToArray()));
    }
}
