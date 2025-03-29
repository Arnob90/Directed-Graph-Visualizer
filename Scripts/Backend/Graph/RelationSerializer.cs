using System;
using Godot;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using RelationSpace; 
using TypeRegistrySpace; 
using Optional; // Library for handling optional values
using Optional.Unsafe; // For Option.ValueOrFailure()

/// <summary>
/// Contains types and logic for serializing Relation objects into an intermediate format.
/// </summary>
namespace RelationSerializerSpace;

/// <summary>
/// Defines an intermediate data structure for storing a serialized relation.
/// This format is designed to be easily convertible to/from final serialization formats like JSON or XML.
/// </summary>
/// <param name="DomainSet">The serialized representation of the relation's domain set.</param>
/// <param name="CodomainSet">The serialized representation of the relation's codomain set.</param>
/// <param name="RelationMapStr">An array of strings, where each string represents a single pair (tuple) in the relation, typically formatted as "(serializedDomainElement,serializedCodomainElement)".</param>
public record IntermediateFormatToSerializeTo(HashSetSerialized DomainSet, HashSetSerialized CodomainSet, String[] RelationMapStr);

/// <summary>
/// Defines the serialized representation of a HashSet.
/// </summary>
/// <param name="SetStr">An array of strings, where each string is the serialized representation of an element in the set.</param>
/// <param name="TypeStr">A string identifier representing the Type of the elements in the set (obtained from TypeSerializationRegistry).</param>
public record HashSetSerialized(String[] SetStr, String TypeStr);

/// <summary>
/// Provides methods to serialize Relation objects
/// to an intermediate string-based format (`IntermediateFormatToSerializeTo`).
/// Includes helper methods for serializing/deserializing HashSets as part of the process.
/// Relies on the `TypeRegistrySpace.TypeSerializationRegistry` to handle serialization/deserialization
/// of individual domain and codomain elements.
/// </summary>
public class RelationSerializer
{
    /// <summary>
    /// Serializes an ImmutableHashSet of type T into a HashSetSerialized object.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    /// <param name="givenSet">The immutable hash set to serialize.</param>
    /// <returns>
    /// An Option containing the HashSetSerialized representation if a serializer for type T is registered
    /// and serialization succeeds for all elements. Otherwise, returns Option.None.
    /// </returns>
    /// <remarks>
    /// - Requires a registered `ITypeSerializer<T>` in `TypeSerializationRegistry`.
    /// - Prints an error using `GD.PrintErr` if the type serializer is not found (tight coupling to Godot).
    /// - Assumes `ITypeSerializer<T>.SerializeFrom` does not throw exceptions for valid inputs.
    /// - Assumes `TypeSerializationRegistry.GetStringReprForType<T>()` exists and returns the correct type identifier string.
    /// </remarks>
    private Option<HashSetSerialized> SerializeHashSetToStr<T>(ImmutableHashSet<T> givenSet)
    {
        var typeSerializer = TypeSerializationRegistry.GetTypeSerializerForType<T>();
        typeSerializer.Match(
                some: (_) => { },
                none: () => GD.PrintErr("Type is not registered") // Note: Godot dependency
        );
        var serializedSetStr = typeSerializer.Map((serializer) =>
        {
            List<String> requiredStrRepr = new();
            foreach (var elem in givenSet)
            {
                requiredStrRepr.Add(serializer.SerializeFrom(elem));
            }
            return requiredStrRepr;
        });
        var serializedSet = serializedSetStr.Map((setRepr) => new HashSetSerialized(setRepr.ToArray(), TypeSerializationRegistry.GetStringReprForType<T>()));
        return serializedSet;
    }

    /// <summary>
    /// Deserializes a HashSetSerialized object back into an ImmutableHashSet of type T.
    /// </summary>
    /// <typeparam name="T">The target type of elements in the set.</typeparam>
    /// <param name="givenSet">The serialized hash set data.</param>
    /// <param name="typeSerializer">The specific ITypeSerializer<T> instance to use for deserialization.</param>
    /// <returns>An ImmutableHashSet<T> containing the deserialized elements.</returns>
    /// <remarks>
    /// - The caller must provide the correct `typeSerializer` instance.
    /// - This method assumes `ITypeSerializer<T>.DeserializeFromString` will succeed for the strings in `givenSet.SetStr`.
    /// - It does not handle potential exceptions (e.g., FormatException) during element deserialization; such exceptions will propagate upwards.
    /// </remarks>
    private ImmutableHashSet<T> DeserializeHashSetFromStr<T>(HashSetSerialized givenSet, ITypeSerializer<T> typeSerializer)
    {
        var requiredSet = ImmutableHashSet<T>.Empty;
        foreach (var elem in givenSet.SetStr)
        {
            // Note: Potential exceptions from DeserializeFromString are not caught here.
            requiredSet = requiredSet.Add(typeSerializer.DeserializeFromString(elem));
        }
        return requiredSet;
    }

    /// <summary>
    /// Dynamically deserializes a HashSetSerialized object when the element type is not known at compile time.
    /// Uses reflection to find and invoke the generic `DeserializeHashSetFromStr<T>` method.
    /// </summary>
    /// <param name="givenSet">The serialized hash set data, containing the TypeStr identifier.</param>
    /// <returns>
    /// An Option containing the deserialized set as an object if deserialization succeeds.
    /// Returns Option.None if the type or its serializer cannot be found or instantiated, or if the reflection lookup fails.
    /// </returns>
    /// <remarks>
    /// - Relies heavily on reflection (`GetMethods`, `MakeGenericMethod`, `Invoke`), which can be slow and brittle (sensitive to method name/signature changes).
    /// - Assumes `TypeSerializationRegistry` can provide the serializer Type and the target element Type based on `givenSet.TypeStr`.
    /// - Uses `Activator.CreateInstance` to create the serializer instance, assuming a parameterless constructor.
    /// - Uses `ValueOrFailure`, which can mask the specific reason for failure if types/serializers are not found by throwing a generic exception.
    /// - Does not handle exceptions that might occur *during* the invoked generic deserialization (e.g., from `DeserializeFromString`).
    /// </remarks>
    private Option<object> DeserializeHashSetFromStr(HashSetSerialized givenSet)
    {
        var typeSerializerT = TypeSerializationRegistry.GetTypeSerializerFromRegisteredStr(givenSet.TypeStr);
        // Assumes serializer has parameterless constructor
        var typeSerializer = typeSerializerT.Map((typeSerializerT) => Activator.CreateInstance(typeSerializerT));
        // Assumes serializer implements ITypeSerializer<T> directly
        var typeToBeSerialized = typeSerializerT.Map((typeSerializerT) => typeSerializerT.GetInterface(typeof(ITypeSerializer<>).Name)?.GetGenericArguments()[0]); // Note: Null check added mentally, original code lacks it

        if (!(typeToBeSerialized.HasValue && typeSerializer.HasValue))
        {
            return Option.None<object>();
        }

        var typeToBeSerializedKnown = typeToBeSerialized.ValueOrFailure(); // Potential generic Exception here
        var typeSerializerKnown = typeSerializer.ValueOrFailure(); // Potential generic Exception here

        // Reflection: Find the generic method based on name and parameter count. Brittle.
        var otherMethodToCall = this.GetType().GetMethods().Where((m) => m.Name == "DeserializeHashSetFromStr" && m.IsGenericMethod && m.GetParameters().Length == 2).First(); // Potential InvalidOperationException if not found

        // Invoke the found generic method.
        return Option.Some<object>(otherMethodToCall.MakeGenericMethod(typeToBeSerializedKnown).Invoke(this, new object[] { givenSet, typeSerializerKnown })); // Potential TargetInvocationException
    }

    /// <summary>
    /// Serializes a Relation<DomainType, CodomainType> object into the intermediate format `IntermediateFormatToSerializeTo`.
    /// </summary>
    /// <typeparam name="DomainType">The type of the relation's domain elements.</typeparam>
    /// <typeparam name="CodomainType">The type of the relation's codomain elements.</typeparam>
    /// <param name="relation">The relation object to serialize.</param>
    /// <returns>
    /// An Option containing the `IntermediateFormatToSerializeTo` if serialization of domain, codomain,
    /// and all relation pairs succeeds. Returns Option.None if serialization of the domain or codomain set fails
    /// (e.g., due to missing type serializers).
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if a registered serializer is found for DomainType or CodomainType, but the
    /// `SerializeFrom` call fails (returns None) for a specific element *within the relation map*.
    /// This indicates an issue with serializing a particular value, not necessarily a missing serializer type.
    /// Note: This exception behavior is inconsistent with the overall Option-based return approach for missing serializers.
    /// </exception>
    /// <remarks>
    /// - Serializes the domain and codomain sets first using `SerializeHashSetToStr`. If either fails, returns None.
    /// - Handles empty relations gracefully by returning the intermediate format with an empty `RelationMapStr`.
    /// - Requires registered serializers for both DomainType and CodomainType in `TypeSerializationRegistry`.
    /// - The format for relation pairs in `RelationMapStr` is "(serializedDomainElement,serializedCodomainElement)". Parsing this requires care during deserialization.
    /// - Uses `ValueOrFailure` after checking `HasValue`, which throws a generic exception if the check was somehow bypassed.
    /// </remarks>
    public Option<IntermediateFormatToSerializeTo> SerializeRelationToIntermediateFormat<DomainType, CodomainType>(Relation<DomainType, CodomainType> relation)
    {
        var domainSerialized = SerializeHashSetToStr<DomainType>(relation.GetDomainSet());
        var codomainSerialized = SerializeHashSetToStr<CodomainType>(relation.GetCodomainSet());

        // If domain or codomain set serialization failed (likely missing serializer), return None.
        if (!domainSerialized.HasValue || !codomainSerialized.HasValue)
        {
            return Option.None<IntermediateFormatToSerializeTo>();
        }

        var domainSerializedKnown = domainSerialized.ValueOrFailure(); // Safe due to above check
        var codomainSerializedKnown = codomainSerialized.ValueOrFailure(); // Safe due to above check

        if (relation.RelationMap.Count == 0) // Assumes RelationMap represents the pairs accurately.
        {
            return Option.Some<IntermediateFormatToSerializeTo>(new IntermediateFormatToSerializeTo(domainSerializedKnown, codomainSerializedKnown, Array.Empty<string>()));
        }

        var domainTypeSerializer = TypeSerializationRegistry.GetTypeSerializerForType<DomainType>();
        var codomainTypeSerializer = TypeSerializationRegistry.GetTypeSerializerForType<CodomainType>();

        var finalRelation = new List<String>();
        // Assumes ConvertToNormalForm yields domain/codomain pairs.
        foreach (var (domainElem, codomainElem) in relation.ConvertToNormalForm())
        {
            // Attempt to serialize individual elements of the pair.
            var domainElemSerialized = domainTypeSerializer.Map((serializer) => serializer.SerializeFrom(domainElem));
            var codomainElemSerialized = codomainTypeSerializer.Map((serializer) => serializer.SerializeFrom(codomainElem));

            // Check if serialization of *this specific pair* failed.
            if (!(domainElemSerialized.HasValue && codomainElemSerialized.HasValue))
            {
                // Throws exception on element serialization failure, inconsistent with Option return for set failure.
                throw new ArgumentException("Type is not registered, or serialization failed");
            }

            // Pair elements serialized successfully.
            var domainElemSerializedKnown = domainElemSerialized.ValueOrFailure(); // Safe due to above check
            var codomainElemSerializedKnown = codomainElemSerialized.ValueOrFailure(); // Safe due to above check

            // Add the formatted pair string. Format requires careful parsing on deserialization.
            finalRelation.Add($"({domainElemSerializedKnown},{codomainElemSerializedKnown})");
        }

        // Return the complete intermediate format object.
        return Option.Some<IntermediateFormatToSerializeTo>(new IntermediateFormatToSerializeTo(domainSerializedKnown, codomainSerializedKnown, finalRelation.ToArray()));
    }

}
