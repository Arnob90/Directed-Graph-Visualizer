using System;
using System.Collections;
using Godot;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace RelationSpace;
/// <summary>
/// Represents an immutable mathematical relation between two sets (Domain and Codomain)
/// </summary>
/// <typeparam name="DomainType" The type of elements in Domain</typeparam>
/// <typeparam name="CodomainType" The type of elements in Codomain</typeparam>
/// <remarks>
/// A relation is a subset of Domain*Codomain. This implemention is immutable and thread safe
/// </remarks>
public record Relation<DomainType, CodomainType> where DomainType : notnull where CodomainType : notnull
{
    //We set it to private and use getter methods instead to avoid with expressions breaking relation
    private ImmutableHashSet<DomainType> DomainSet { get; init; }
    private ImmutableHashSet<CodomainType> CodomainSet { get; init; }
    private ImmutableDictionary<DomainType, ImmutableHashSet<CodomainType>> RelationMap { get; init; }
    public Relation(IDictionary<DomainType, ImmutableHashSet<CodomainType>> relationMap, ImmutableHashSet<DomainType> domainSet, ImmutableHashSet<CodomainType> codomainSet)
    {
        if (!relationMap.All((pair) => domainSet.Contains(pair.Key) && pair.Value.All((mappedToValues) => codomainSet.Contains(mappedToValues))))
        {
            throw new ArgumentException("The relation set must be subset of X*Y");
        }
        CodomainSet = codomainSet;
        DomainSet = domainSet;
        RelationMap = relationMap.ToImmutableDictionary();
    }
    ///<summary>Get the domain set of the relation</summary>
    public ImmutableHashSet<DomainType> GetDomainSet()
    {
        return DomainSet;
    }
    ///<summary> Get the codomain set of the relation</summary>
    public ImmutableHashSet<CodomainType> GetCodomainSet()
    {
        return CodomainSet;
    }
    public Relation(ImmutableHashSet<(DomainType, CodomainType)> relationSet, ImmutableHashSet<DomainType> domainSet, ImmutableHashSet<CodomainType> codomainSet) : this(BuildRelationMap(relationSet), domainSet, codomainSet)
    {
    }
    private static ImmutableDictionary<DomainType, ImmutableHashSet<CodomainType>> BuildRelationMap(ImmutableHashSet<(DomainType, CodomainType)> relationSet)
    {
        var relationMap = ImmutableDictionary<DomainType, ImmutableHashSet<CodomainType>>.Empty;
        foreach (var (elem1, elem2) in relationSet)
        {
            relationMap = AddEdgeToRelationDict(relationMap, (elem1, elem2));
        }
        return relationMap;
    }
    /// <summary>Given a dictionary where the key indicates an element and the value is the set of all elements it is related to, convert it to a set of ordered pairs(standard form of relation)</summary>
    /// <param name="mapForm"> A dictionary of which each key is mapped to everything it is related to. </param>
    public static IEnumerable<(DomainType, CodomainType)> ConvertRelationMapToNormalForm(IDictionary<DomainType, ImmutableHashSet<CodomainType>> mapForm)
    {
        var relationSet = ImmutableHashSet<(DomainType, CodomainType)>.Empty;
        foreach (var (elem1, mappedToValues) in mapForm)
        {
            foreach (var node in mappedToValues)
            {
                yield return (elem1, node);
            }
        }
    }

    public IEnumerable<(DomainType, CodomainType)> ConvertToNormalForm()
    {
        return ConvertRelationMapToNormalForm(RelationMap);
    }
    public Relation<CodomainType, DomainType> GetInverseRelation()
    {
        return new Relation<CodomainType, DomainType>(ConvertToNormalForm().Select((pair) => (pair.Item2, pair.Item1)).ToImmutableHashSet(), CodomainSet, DomainSet);
    }
    public bool IsRelatedTo(DomainType firstElem, CodomainType secondElem)
    {
        if (!(DomainSet.Contains(firstElem) && CodomainSet.Contains(secondElem)))
        {
            throw new ArgumentException("This operation is undefined because either the first or second element is not present in domain or codomain respectively");
        }
        ImmutableHashSet<CodomainType> mappedToNodes;
        if (!RelationMap.TryGetValue(firstElem, out mappedToNodes))
        {
            return false;
        }
        return mappedToNodes.Contains(secondElem);
    }
    private static ImmutableDictionary<DomainType, ImmutableHashSet<CodomainType>> AddEdgeToRelationDict(ImmutableDictionary<DomainType, ImmutableHashSet<CodomainType>> relationDict, (DomainType, CodomainType) pair)
    {
        var (elem1, elem2) = pair;
        GD.Print($"{{{elem1},{elem2}}}");
        if (relationDict.ContainsKey(elem1))
        {
            var val = relationDict[elem1];
            if (!val.Contains(elem2))
            {
                return relationDict.SetItem(elem1, val.Add(elem2));
            }
            return relationDict;
        }
        return relationDict.Add(elem1, ImmutableHashSet.Create<CodomainType>(elem2));
    }
    public Relation<DomainType, CodomainType> AddEdge((DomainType, CodomainType) pair)
    {
        if (!DomainSet.Contains(pair.Item1) || !CodomainSet.Contains(pair.Item2))
        {
            throw new ArgumentException("The pair to be added isn't part of Domain*Codomain");
        }
        return this with { RelationMap = AddEdgeToRelationDict(RelationMap, pair) };
    }
    public Relation<DomainType, CodomainType> RemoveEdge((DomainType, CodomainType) pair)
    {
        if (!RelationMap.ContainsKey(pair.Item1))
        {
            throw new ArgumentException("Cannot remove an edge that doesn't even exist");
        }
        return new Relation<DomainType, CodomainType>(ConvertToNormalForm().Where((relationExpr) => EqualityComparer<(DomainType, CodomainType)>.Default.Equals(pair, relationExpr)).ToImmutableHashSet(), DomainSet, CodomainSet);
    }
    public ImmutableHashSet<CodomainType> GetNeighbours(DomainType elem)
    {
        if (DomainSet.Contains(elem) && !RelationMap.ContainsKey(elem))
        {
            return ImmutableHashSet<CodomainType>.Empty;
        }
        return RelationMap[elem];
    }
    public bool HasEdge((DomainType, CodomainType) edge)
    {
        return ConvertToNormalForm().Contains(edge);
    }
    public override string ToString()
    {
        var relationSet = ConvertToNormalForm();
        var finalStr = "{";
        foreach (var pair in relationSet)
        {
            finalStr += ($"({pair.Item1},{pair.Item2}),");
        }
        finalStr = finalStr[..^1];
        finalStr += "}";
        finalStr = finalStr.Trim();
        return finalStr;
    }
}
