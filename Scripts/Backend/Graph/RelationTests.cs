using UltraUnitTesterSpace;
using UltraUnitTesterSpace.ResultTypesSpace;
using RelationParserSpace;
using Godot;
[TestClass]
class RelationTestClass
{
    [TestMethod]
    public static ResultType TestRelationParsing()
    {
        var parse1 = BasicRelationParser.ParseFromString("{(1,2)}", System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 4));
        GD.Print(parse1.ToString());
        if (parse1.HasEdge((1, 2)))
        {
            return new SuccessType();
        }
        return new FailureType();
    }
}
