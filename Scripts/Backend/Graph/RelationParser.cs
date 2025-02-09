using System;
using System.Linq;
using Godot;
using System.Collections.Generic;
using System.Collections.Immutable;
using RelationSpace;
namespace RelationParserSpace;
public class BasicRelationParser
{
    public Relation<int, int> ParseFromString(String givenRelationStr,ImmutableHashSet<int> domainSet)
    {
        givenRelationStr = givenRelationStr.Trim();
        if (givenRelationStr[0] != '{' || givenRelationStr[givenRelationStr.Length - 1] != '}')
        {
            throw new ArgumentException("A relation string must be of form {...}");
        }
        givenRelationStr = givenRelationStr.Remove(0).Remove(givenRelationStr.Length - 1);
        givenRelationStr = givenRelationStr.Trim();
        var parsedRelationsStr = new List<String>();
        var toParseStr = givenRelationStr;
        while (toParseStr != "")
        {
			toParseStr=toParseStr.Trim();
            var relationStart = toParseStr.IndexOf('(');
            var relationEnd = toParseStr.IndexOf(')');
            parsedRelationsStr.Add(toParseStr.Substring(relationStart, relationEnd-relationStart+1));
            toParseStr = toParseStr.Substring(relationEnd + 1, toParseStr.Length);
        }
		var parsedRelations=parsedRelationsStr.Select((relationStr)=>relationStr.Trim().Split(','));
		var finalResultSet=ImmutableHashSet<(int,int)>.Empty;
		foreach(var parsedRelation in parsedRelations)
		{
			//Some input validation goes a long way
			if(parsedRelation.Length!=2)
			{
				throw new ArgumentException("ParseFromString method only supports binary relations!");
			}
			var firstIntInPairStr=parsedRelation[0];
			var secondIntInPairStr=parsedRelation[1];
			int firstIntInPair=0;
			int secondIntInPair=0;
			if(!int.TryParse(firstIntInPairStr,out firstIntInPair)||!int.TryParse(secondIntInPairStr,out secondIntInPair))
			{
				throw new ArgumentException("The given input is not of int");
			}
			(int,int) requiredPair=(firstIntInPair,secondIntInPair);
			if(finalResultSet.Contains((requiredPair)))
			{
				GD.PrintErr("Duplicate pair in input ignored");
			}
			finalResultSet=finalResultSet.Add(requiredPair);
		}
		return new Relation<int, int>(finalResultSet,domainSet,domainSet);
    }
}
