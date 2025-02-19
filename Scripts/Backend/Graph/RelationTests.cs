using UltraUnitTesterSpace;
using UltraUnitTesterSpace.ResultTypesSpace;
using System.Linq;
using RelationParserSpace;
using Godot;
using System;
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

    [TestMethod]
    public static ResultType TestMultiplePairs()
    {
        var parse = BasicRelationParser.ParseFromString("{(1,2), (2,4), (4,1)}", System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 4));
        if (parse.HasEdge((1, 2)) && parse.HasEdge((2, 4)) && parse.HasEdge((4, 1)))
        {
            return new SuccessType();
        }
        return new FailureType();
    }

    [TestMethod]
    public static ResultType TestDuplicatePairs()
    {
        var parse = BasicRelationParser.ParseFromString("{(1,2), (1,2)}", System.Collections.Immutable.ImmutableHashSet.Create(1, 2));
        if (parse.GetNeighbours(1).Count == 1) // Ensure duplicates are ignored
        {
            return new SuccessType();
        }
        return new FailureType();
    }

    [TestMethod]
    public static ResultType TestInvalidFormatMissingBrace()
    {
        try
        {
            var parse = BasicRelationParser.ParseFromString("(1,2)}", System.Collections.Immutable.ImmutableHashSet.Create(1, 2));
            return new FailureType("Expected ArgumentException but no exception was thrown");
        }
        catch (ArgumentException)
        {
            return new SuccessType();
        }
    }

    [TestMethod]
    public static ResultType TestInvalidFormatMissingParenthesis()
    {
        try
        {
            var parse = BasicRelationParser.ParseFromString("{{1,2}}", System.Collections.Immutable.ImmutableHashSet.Create(1, 2));
            return new FailureType("Expected FormatException but no exception was thrown");
        }
        catch (FormatException)
        {
            return new SuccessType();
        }
    }

    [TestMethod]
    public static ResultType TestInvalidFormatNonIntegerInput()
    {
        try
        {
            var parse = BasicRelationParser.ParseFromString("{(1,abc)}", System.Collections.Immutable.ImmutableHashSet.Create(1, 2));
            return new FailureType("Expected ArgumentException but no exception was thrown");
        }
        catch (ArgumentException)
        {
            return new SuccessType();
        }
    }

    [TestMethod]
    public static ResultType TestInvalidFormatNonBinaryRelation()
    {
        try
        {
            var parse = BasicRelationParser.ParseFromString("{(1,2,3)}", System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 3));
            return new FailureType("Expected ArgumentException but no exception was thrown");
        }
        catch (ArgumentException)
        {
            return new SuccessType();
        }
    }

    [TestMethod]
    public static ResultType TestEmptyRelation()
    {
        var parse = BasicRelationParser.ParseFromString("{}", System.Collections.Immutable.ImmutableHashSet.Create(1, 2));
        if (parse.GetDomainSet().Count == 2 && parse.ConvertToNormalForm().Count() == 0)
        {
            return new SuccessType();
        }
        return new FailureType();
    }

    [TestMethod]
    public static ResultType TestWhitespaceTolerance()
    {
        var parse = BasicRelationParser.ParseFromString("{ (1 , 2) , ( 2 , 4 ) }", System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 4));
        if (parse.HasEdge((1, 2)) && parse.HasEdge((2, 4)))
        {
            return new SuccessType();
        }
        return new FailureType();
    }

    [TestMethod]
    public static ResultType TestInvalidDomainElement()
    {
        try
        {
            var parse = BasicRelationParser.ParseFromString("{(1,2), (3,4)}", System.Collections.Immutable.ImmutableHashSet.Create(1, 2));
            return new FailureType("Expected ArgumentException but no exception was thrown");
        }
        catch (ArgumentException)
        {
            return new SuccessType();
        }
    }
}
