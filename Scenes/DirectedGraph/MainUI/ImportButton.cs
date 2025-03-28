using Godot;
using System;
using System.Collections.Immutable;
using MainUISpace;
using RelationParserSpace;
using NodeContainerSpace;
public partial class ImportButton : Button
{
    [Export]
    LineEdit LabelToImportFrom;
    [Export]
    LineEdit DomainSetLabel;
    [Export]
    NodeContainer ToContainDiagramNodeIn;
    public override void _Ready()
    {
        Pressed += ImportRelationFromText;
    }

    public override void _Process(double delta)
    {
    }

    private void ImportRelationFromText()
    {
        if (LabelToImportFrom.Text == "")
        {
            return;
        }
        var parsedRelation = BasicRelationParser.ParseFromString(LabelToImportFrom.Text, BasicRelationParser.ParseDomainSetFromStr(DomainSetLabel.Text).ToImmutableHashSet());
        ToContainDiagramNodeIn.DisplayRelation(parsedRelation);
    }
}
