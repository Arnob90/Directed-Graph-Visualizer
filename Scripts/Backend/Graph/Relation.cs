using System;
using System.Collections;
using Godot;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RelationSpace;

/// <summary>
/// Represents an immutable mathematical relation between two sets (Domain and Codomain).
/// </summary>
/// <typeparam name="DomainType">The type of elements in the Domain.</typeparam>
/// <typeparam name="CodomainType">The type of elements in the Codomain.</typeparam>
/// <remarks>
/// A relation is a subset of Domain * Codomain. This implementation is immutable and thread-safe.
/// </remarks>
public record Relation<DomainType, CodomainType> where DomainType : notnull where CodomainType : notnull
{
    // We set it to private and use getter methods instead to avoid with expressions breaking relation
    private ImmutableHashSet<DomainType> DomainSet { get; init; }
    private ImmutableHashSet<CodomainType> CodomainSet { get; init; }
    public ImmutableDictionary<DomainType, ImmutableHashSet<CodomainType>> RelationMap { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Relation{DomainType, CodomainType}"/> class.
    /// </summary>
    /// <param name="relationMap">A dictionary representing the relation map.</param>
    /// <param name="domainSet">The domain set of the relation.</param>
    /// <param name="codomainSet">The codomain set of the relation.</param>
    /// <exception cref="ArgumentException">Thrown when the relation set is not a subset of Domain * Codomain.</exception>
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

    /// <summary>
    /// Gets the domain set of the relation.
    /// </summary>
    /// <returns>The domain set.</returns>
    public ImmutableHashSet<DomainType> GetDomainSet()
    {
        return DomainSet;
    }

    /// <summary>
    /// Gets the codomain set of the relation.
    /// </summary>
    /// <returns>The codomain set.</returns>
    public ImmutableHashSet<CodomainType> GetCodomainSet()
    {
        return CodomainSet;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Relation{DomainType, CodomainType}"/> class.
    /// </summary>
    /// <param name="relationSet">A set of tuples representing the relation.</param>
    /// <param name="domainSet">The domain set of the relation.</param>
    /// <param name="codomainSet">The codomain set of the relation.</param>
    public Relation(ImmutableHashSet<(DomainType, CodomainType)> relationSet, ImmutableHashSet<DomainType> domainSet, ImmutableHashSet<CodomainType> codomainSet)
        : this(BuildRelationMap(relationSet), domainSet, codomainSet)
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

    /// <summary>
    /// Converts a dictionary representation of a relation to a set of ordered pairs (standard form of relation).
    /// </summary>
    /// <param name="mapForm">A dictionary where each key is mapped to a set of elements it is related to.</param>
    /// <returns>An enumerable of tuples representing the relation in normal form.</returns>
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

    /// <summary>
    /// Converts the current relation to its normal form (a set of ordered pairs).
    /// </summary>
    /// <returns>An enumerable of tuples representing the relation in normal form.</returns>
    public IEnumerable<(DomainType, CodomainType)> ConvertToNormalForm()
    {
        return ConvertRelationMapToNormalForm(RelationMap);
    }

    /// <summary>
    /// Gets the inverse of the current relation.
    /// </summary>
    /// <returns>A new <see cref="Relation{CodomainType, DomainType}"/> representing the inverse relation.</returns>
    public Relation<CodomainType, DomainType> GetInverseRelation()
    {
        return new Relation<CodomainType, DomainType>(ConvertToNormalForm().Select((pair) => (pair.Item2, pair.Item1)).ToImmutableHashSet(), CodomainSet, DomainSet);
    }

    /// <summary>
    /// Checks if a given pair of elements are related in the current relation.
    /// </summary>
    /// <param name="firstElem">The element from the domain.</param>
    /// <param name="secondElem">The element from the codomain.</param>
    /// <returns>True if the elements are related, otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when either element is not present in the domain or codomain.</exception>
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

    /// <summary>
    /// Adds an edge (pair) to the relation.
    /// </summary>
    /// <param name="pair">The pair to add to the relation.</param>
    /// <returns>A new <see cref="Relation{DomainType, CodomainType}"/> with the added edge.</returns>
    /// <exception cref="ArgumentException">Thrown when the pair to be added is not part of Domain * Codomain.</exception>
    public Relation<DomainType, CodomainType> AddEdge((DomainType, CodomainType) pair)
    {
        if (!DomainSet.Contains(pair.Item1) || !CodomainSet.Contains(pair.Item2))
        {
            throw new ArgumentException("The pair to be added isn't part of Domain*Codomain");
        }
        return this with { RelationMap = AddEdgeToRelationDict(RelationMap, pair) };
    }

    /// <summary>
    /// Removes an edge (pair) from the relation.
    /// </summary>
    /// <param name="pair">The pair to remove from the relation.</param>
    /// <returns>A new <see cref="Relation{DomainType, CodomainType}"/> with the edge removed.</returns>
    /// <exception cref="ArgumentException">Thrown when the edge to be removed does not exist.</exception>
    public Relation<DomainType, CodomainType> RemoveEdge((DomainType, CodomainType) pair)
    {
        if (!RelationMap.ContainsKey(pair.Item1))
        {
            throw new ArgumentException("Cannot remove an edge that doesn't even exist");
        }
        return new Relation<DomainType, CodomainType>(ConvertToNormalForm().Where((relationExpr) => EqualityComparer<(DomainType, CodomainType)>.Default.Equals(pair, relationExpr)).ToImmutableHashSet(), DomainSet, CodomainSet);
    }

    /// <summary>
    /// Gets the set of elements in the codomain that are related to a given element in the domain.
    /// </summary>
    /// <param name="elem">The element in the domain.</param>
    /// <returns>An immutable hash set of elements in the codomain that are related to the given element.</returns>
    public ImmutableHashSet<CodomainType> GetNeighbours(DomainType elem)
    {
        if (DomainSet.Contains(elem) && !RelationMap.ContainsKey(elem))
        {
            return ImmutableHashSet<CodomainType>.Empty;
        }
        return RelationMap[elem];
    }

    /// <summary>
    /// Checks if a given edge (pair) exists in the relation.
    /// </summary>
    /// <param name="edge">The edge to check.</param>
    /// <returns>True if the edge exists in the relation, otherwise false.</returns>
    public bool HasEdge((DomainType, CodomainType) edge)
    {
        return ConvertToNormalForm().Contains(edge);
    }
    /// <summary>
    /// Return the amount of edges in the relation
    /// </summary>
    /// <returns>
    /// An int representing the amount of edges in the relation
    /// </returns>
    public int Count()
    {
        return ConvertToNormalForm().Count();
    }

    /// <summary>
    /// Returns a string representation of the relation.
    /// </summary>
    /// <returns>A string representing the relation in normal form.</returns>
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
