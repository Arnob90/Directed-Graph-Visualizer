using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using MiscSpace;
using System.Collections;
using RelationSpace;
using DiagramNodeSpace;
using RelationParserSpace;
using DirectionalLineSpace;
namespace MainUISpace;
public partial class MainUi : Node2D
{
    [Export]
    PackedScene DiagramNodeScene;
    [Export]
    float DistanceFromNodes = 60;
    [Export]
    Vector2 PositionOfFirstNode = new(120, 60);
    [Export]
    PackedScene DirectionalLineScene;
    public void CreateDiagramNodesFromRelation<DomainType>(Relation<DomainType, DomainType> givenRelation)
    {
        var domain = givenRelation.GetDomainSet();
        var requiredNodes = new List<DiagramNode>();
        var stringReprToNodeMap = new Dictionary<String, DiagramNode>();
        var nodeToStringReprMap = new Dictionary<DiagramNode, String>();
        var stringReprToPairMap = new Dictionary<String, DomainType>();
        var pairToStringReprMap = new Dictionary<DomainType, String>();
        foreach (var elem in domain)
        {
            var requiredDiagramNode = DiagramNodeScene.Instantiate<DiagramNode>();
            var elemStrRepr = elem.ToString();
            requiredDiagramNode.TextLabel.Text = elem.ToString();
            requiredNodes.Add(requiredDiagramNode);
            stringReprToPairMap.Add(elemStrRepr, elem);
            stringReprToNodeMap.Add(elemStrRepr, requiredDiagramNode);
        }
        nodeToStringReprMap = Misc.GetInverseDict(stringReprToNodeMap);
        pairToStringReprMap = Misc.GetInverseDict(stringReprToPairMap);
        var numberOfChildren = GetChildCount();
        DistributeDiagramNodes(requiredNodes);
        var relationSet = givenRelation.ConvertToNormalForm();
        foreach (var (from, to) in relationSet)
        {
            var edgeLine = DirectionalLineScene.Instantiate<DirectionalLine>();
            var fromStringRepr = from.ToString();
            var toStringRepr = to.ToString();
            var fromNode = stringReprToNodeMap[fromStringRepr];
            var toNode = stringReprToNodeMap[toStringRepr];
            if (fromNode == toNode)
            {
                fromNode.MappedToSelf = true;
                continue;
            }
            var fromNodePosition = fromNode.CenterAnchor.GlobalPosition;
            var toNodePosition = toNode.CenterAnchor.GlobalPosition;
            edgeLine.Points = new Vector2[] { fromNodePosition, toNodePosition };
            AddChild(edgeLine);
            //The line must be below the node themselves to give the illusion of them 
            MoveChild(edgeLine, numberOfChildren);
            ++numberOfChildren;
        }
    }
    private void DistributeDiagramNodes(List<DiagramNode> nodesToDistribute)
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
        }
    }
    public override void _Ready()
    {
        GD.Print(System.Environment.ProcessId);
        // CreateDiagramNodesFromRelation(BasicRelationParser.ParseFromString("{(1,2),(3,4),(7,7)}", ImmutableHashSet.Create(1, 2, 3, 4, 7)));
    }
}
