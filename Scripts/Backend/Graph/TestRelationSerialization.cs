using System;
using Godot;
using Optional;
using Newtonsoft.Json;
using Optional.Unsafe;
using RelationSerializerSpace;
using System.Collections.Immutable;
using RelationSpace;
using UltraUnitTesterSpace;
using UltraUnitTesterSpace.ResultTypesSpace;
[TestClass]
public partial class RelationSerializationTests
{
    [TestMethod]
    public ResultType TestRelationSerialization()
    {
        var serializer = new RelationSerializer();
        var relationSet = ImmutableHashSet.Create<(int, int)>((2, 3), (4, 6), (2, 2), (7, 8));
        var relationToSerialize = new Relation<int, int>(relationSet, ImmutableHashSet.Create<int>(1, 2, 3, 4, 5, 6, 7, 8), ImmutableHashSet.Create(1, 2, 3, 4, 5, 6, 7, 8));
        var serializedRelation = serializer.SerializeRelationToIntermediateFormat(relationToSerialize);
        if (!serializedRelation.HasValue)
        {
            return new FailureType();
        }
        var serializedRelationKnown = serializedRelation.ValueOrFailure();
        var serializationJson = JsonConvert.SerializeObject(serializedRelationKnown, Formatting.Indented);
        GD.Print(serializationJson);
        return new SuccessType();
    }
}
