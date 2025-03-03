using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections;
using RelationSpace;
abstract record GraphNode;
record NormalGraphNode<T>(T elem) : GraphNode where T : notnull
{
}
record WeightedGraphNode<T>(T elem, double weight) : GraphNode where T : notnull;
record DomainType<T>(HashSet<T> DomainSet) : IEnumerable<T> where T : notnull
{
    public IEnumerator<T> GetEnumerator()
    {
        foreach (var elem in DomainSet)
        {
            yield return elem;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
///<summary>A directed graph of a relation over a Domain. Permits loops</summary>
record DirectedGraph<T> where T : GraphNode
{
    Relation<T, T> UnderlyingRelation { get; init; }
    public DirectedGraph(Relation<T, T> givenRelation)
    {
        UnderlyingRelation = givenRelation;
    }
    public DirectedGraph<T> AddEdge((T, T) givenNode)
    {
        return this with { UnderlyingRelation = UnderlyingRelation.AddEdge(givenNode) };
    }
    public Relation<T, T> GetRelation()
    {
        return UnderlyingRelation;
    }
    public DirectedGraph<T> RemoveEdge((T, T) givenNode)
    {
        return this with { UnderlyingRelation = UnderlyingRelation.RemoveEdge(givenNode) };
    }
    // public DirectedGraph<T> RemoveNode(GraphNode givenNode)
    // {
    // }
    public bool HasEdge((T, T) edge)
    {
        return UnderlyingRelation.HasEdge(edge);
    }
    public IEnumerable<T> GetNeighbours(T node)
    {
        return UnderlyingRelation.GetNeighbours(node);
    }
}
record DirectedGraphOperations
{
    public static bool CheckReflexivity<T>(DirectedGraph<WeightedGraphNode<T>> graph) where T : struct
    {
        foreach (var elem in graph.GetRelation().GetDomainSet())
        {
            if (!graph.HasEdge((elem, elem)))
            {
                return false;
            }
        }
        return true;
    }
    public static bool CheckAntiSymmetry<T>(DirectedGraph<WeightedGraphNode<T>> graph) where T : struct
    {
        foreach (var (node1, node2) in graph.GetRelation().ConvertToNormalForm())
        {
            if (graph.HasEdge((node2, node1)))
            {
                return false;
            }
        }
        return true;
    }
    public static bool Symmetry<T>(DirectedGraph<WeightedGraphNode<T>> graph) where T : struct
    {
        foreach (var (node1, node2) in graph.GetRelation().ConvertToNormalForm())
        {
            if (!graph.HasEdge((node2, node1)))
            {
                return false;
            }
        }
        return true;
    }
    public static bool CheckTransivity<T>(DirectedGraph<WeightedGraphNode<T>> graph) where T : struct
    {
        var inverseRelation = new Dictionary<WeightedGraphNode<T>, WeightedGraphNode<T>>();
        foreach (var (node1, node2) in graph.GetRelation().ConvertToNormalForm())
        {
            if (inverseRelation.ContainsKey(node1))
            {
                var firstElem = inverseRelation[node1];
                if (!graph.HasEdge((firstElem, node2)))
                {
                    return false;
                }
            }
            inverseRelation.Add(node2, node1);
        }
        return true;
    }
}
record HasseDiagram<T> where T : struct
{
    DirectedGraph<WeightedGraphNode<T>> UnderlyingGraph { get; init; }
    public HasseDiagram(DirectedGraph<WeightedGraphNode<T>> underlyingGraph)
    {
        if (!(DirectedGraphOperations.CheckReflexivity(underlyingGraph) && DirectedGraphOperations.CheckAntiSymmetry(underlyingGraph) && DirectedGraphOperations.CheckTransivity(underlyingGraph)))
        {
            throw new ArgumentException("HasseDiagram must be of a poset");
        }
        UnderlyingGraph = underlyingGraph;
    }
}
