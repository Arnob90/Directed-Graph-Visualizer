using Godot;
using System;
using System.Linq;
using Optional;
using System.Collections.Generic;
using System.Collections.Immutable;
using DirectionalLineSpace;
using DiagramNodeSpace;
using RelationSpace;
using MiscSpace;
namespace NodeContainerSpace;
public partial class NodeContainer : Node2D
{
    [Export]
    PackedScene DiagramNodeScene;
    [Export]
    float DistanceFromNodes = 60;
    [Export]
    Vector2 PositionOfFirstNode = new(120, 60);
    [Export]
    PackedScene DirectionalLineScene;
    [Export]
    Node2D LineContainer;
	DiagramNode SelectedNode;
    public List<DiagramNode> CreateDiagramNodesFromRelation<DomainType>(Relation<DomainType, DomainType> givenRelation)
    {
        var domain = givenRelation.GetDomainSet();
        var relationMap = givenRelation.RelationMap;
        var diagramNodes = new List<DiagramNode>();
        var nodeToElemMap = new Dictionary<DiagramNode, DomainType>();
        var elemToNodeMap = Misc.GetInverseDict(nodeToElemMap);
        foreach (var elem in domain)
        {
            var elemStr = elem.ToString();
            var diagramNode = DiagramNodeScene.Instantiate<DiagramNode>();
            diagramNode.StrRepr = elemStr;
            diagramNodes.Add(diagramNode);
            nodeToElemMap.Add(diagramNode, elem);
            elemToNodeMap.Add(elem, diagramNode);
        }
        var diagramMap = new Dictionary<DiagramNode, HashSet<DiagramNode>>();
        foreach (var node in diagramNodes)
        {
            var elemOfNode = nodeToElemMap[node];
            var neighbourNodes = givenRelation.GetNeighbours(elemOfNode).Select((elem) => elemToNodeMap[elem]).ToHashSet();
            diagramMap.Add(node, neighbourNodes);
            node.Neighbours = neighbourNodes;
        }
        return diagramNodes;
    }
    private void DistributeDiagramNodes(IEnumerable<DiagramNode> nodesToDistribute)
    {
        var totalViewportDimensions = GetViewportRect();
        float initialXDistance = PositionOfFirstNode.X;
        float accumulatedXDistance = initialXDistance;
        float yLevelToDistributeNodes = PositionOfFirstNode.Y;
        foreach (var node in nodesToDistribute)
        {
            AddChild(node);
            if (accumulatedXDistance > totalViewportDimensions.Size.X)
            {
                if (accumulatedXDistance == initialXDistance)
                {
                    node.Position = new Vector2(accumulatedXDistance, yLevelToDistributeNodes);
                }
                yLevelToDistributeNodes += (node.TotalSize.Y + DistanceFromNodes);
                accumulatedXDistance = initialXDistance;
            }
            node.Position = new Vector2(accumulatedXDistance, yLevelToDistributeNodes);
            accumulatedXDistance += (node.TotalSize.X + DistanceFromNodes);
			node.MouseClicked+=HandleSelection;
        }
    }
    private void DrawLinesBetweenNodes(IEnumerable<DiagramNode> diagramNodes)
    {
		foreach(var line in LineContainer.GetChildren())
		{
			line.QueueFree();
		}
        foreach (var node in diagramNodes)
        {
            var neighbours = node.Neighbours;
            foreach (var neighbour in neighbours)
            {
                var line = DirectionalLineScene.Instantiate<DirectionalLine>();
                var centrePointOfNode = node.CenterAnchor;
                var centrePointOfNeighbour = neighbour.CenterAnchor;
                if (neighbour==node)
                {
                    node.MappedToSelf = true;
                    continue;
                }
                line.Points = new Vector2[] { centrePointOfNode.GlobalPosition, centrePointOfNeighbour.GlobalPosition };
				if(neighbour.Neighbours.Contains(node))
				{
					line.Info = GD.Load<BidirectionalLineInfo>("uid://cx3anke0uswqj");
				}
				else
				{
					line.Info = GD.Load<SingleDirectionalLineInfo>("uid://t5ax0ovg0p37");
				}
                LineContainer.AddChild(line);
            }
        }
    }
    public void DisplayRelation<DomainType>(Relation<DomainType, DomainType> givenRelation)
    {
		foreach(var child in GetChildren())
		{
			if(child is DiagramNode)
			{
				child.QueueFree();
			}
		}
        var nodesToDisplay = CreateDiagramNodesFromRelation(givenRelation);
        DistributeDiagramNodes(nodesToDisplay);
        DrawLinesBetweenNodes(nodesToDisplay);
		foreach(var node in nodesToDisplay)
		{
			node.Moving+=()=>{DrawLinesBetweenNodes(nodesToDisplay);};
		}
	}
	private void HandleSelection(DiagramNode nodeSelected)
	{
		var overlappingNodes=nodeSelected.OverlappingNodes;
		var mouseHoveringNodes=overlappingNodes.Where((node)=>node.IsHovering).ToImmutableList().Add(nodeSelected);
		var topMostNode=FindTopMostNode(mouseHoveringNodes);
		topMostNode.Map((node)=>
		{
			((DiagramNode)node).Selected=true;
			return node;
		});
	}
	private Option<Node2D> FindTopMostNode(IEnumerable<Node2D> givenNodes)
	{
		var topMostNode=givenNodes.Count()!=0?Option.Some(givenNodes.First()):Option.None<Node2D>();
		//We already took the first element
		foreach(var node in givenNodes.Skip(1))
		{
			//If nodes aren't siblings, the comparison is useless
			//Two nodes are siblings if and only if they share a common parent, or they are the same root node
			if(!topMostNode.Map((topNode)=>node.GetParent()==topNode.GetParent()).ValueOr(false))
			{
				return Option.None<Node2D>();
			}
			topMostNode=topMostNode.Map((topNode)=>topNode.IsGreaterThan(node)?topNode:node);
		}
		return topMostNode;
	}
}
