using System;
using System.Linq;
using Godot;
using System.Collections.Generic;
using System.Collections.Immutable;
using RelationSpace;
namespace RelationParserSpace;
public class BasicRelationParser
{
    public static Relation<int, int> ParseFromString(String givenRelationStr, ImmutableHashSet<int> domainSet)
    {
        givenRelationStr = givenRelationStr.Trim();
        if (givenRelationStr[0] != '{' || givenRelationStr[givenRelationStr.Length - 1] != '}')
        {
            throw new ArgumentException("A relation string must be of form {...}");
        }
        givenRelationStr = givenRelationStr[1..^1];
        givenRelationStr = givenRelationStr.Trim();
        var parsedRelationsStr = new List<String>();
        var toParseStr = givenRelationStr;
        while (toParseStr != "")
        {
            toParseStr = toParseStr.Trim();
            var relationStart = toParseStr.IndexOf('(');
            var relationEnd = toParseStr.IndexOf(')');
            if (relationStart == -1 | relationEnd == -1)
            {
                throw new FormatException("Invalid string format. Possibly missing ( or )");
            }
            parsedRelationsStr.Add(toParseStr[(relationStart + 1)..relationEnd]);
            toParseStr = toParseStr[(relationEnd + 1)..];
        }
        var parsedRelations = parsedRelationsStr.Select((relationStr) => relationStr.Trim().Split(','));
        var finalResultSet = ImmutableHashSet<(int, int)>.Empty;
        foreach (var parsedRelation in parsedRelations)
        {
            //Some input validation goes a long way
            if (parsedRelation.Length != 2)
            {
                throw new ArgumentException("ParseFromString method only supports binary relations!");
            }
            var firstIntInPairStr = parsedRelation[0];
            var secondIntInPairStr = parsedRelation[1];
            int firstIntInPair = 0;
            int secondIntInPair = 0;
            if (!int.TryParse(firstIntInPairStr, out firstIntInPair) || !int.TryParse(secondIntInPairStr, out secondIntInPair))
            {
                throw new ArgumentException("The given input is not of int");
            }
            (int, int) requiredPair = (firstIntInPair, secondIntInPair);
            if (finalResultSet.Contains((requiredPair)))
            {
                GD.PrintErr("Duplicate pair in input ignored");
            }
            finalResultSet = finalResultSet.Add(requiredPair);
        }
        return new Relation<int, int>(finalResultSet, domainSet, domainSet);
    }
    public static HashSet<int> ParseDomainSetFromStr(String domainSetString)
    {
        var trimmedDomainSetStr = domainSetString.Trim();
        if (domainSetString[0] != '{' && domainSetString[domainSetString.Length - 1] != '}')
        {
            throw new ArgumentException("Domain set must be of form: {a1, a2, a3...,an} for some ints a1,a2,a3...,an");
        }
        var requiredSet = new HashSet<int>();
        var domainSetRemovedBraces = domainSetString[1..^1];
        var domainSetSplit = domainSetRemovedBraces.Split(',');
        foreach (String elemStr in domainSetSplit)
        {
            var elemStrTrimmed = elemStr.Trim();
            var elemNum = int.Parse(elemStrTrimmed);
            requiredSet.Add(elemNum);
        }
        return requiredSet;
    }
}
