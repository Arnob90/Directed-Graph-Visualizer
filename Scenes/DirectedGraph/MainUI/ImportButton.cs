using Godot;
using System;
using System.Collections.Immutable;
using MainUISpace;
using RelationParserSpace;
public partial class ImportButton : Button
{
    [Export]
    LineEdit LabelToImportFrom;
    [Export]
    LineEdit DomainSetLabel;
    [Export]
    MainUi MainUiWindow;
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
        MainUiWindow.CreateDiagramNodesFromRelation(BasicRelationParser.ParseFromString(LabelToImportFrom.Text, BasicRelationParser.ParseDomainSetFromStr(DomainSetLabel.Text).ToImmutableHashSet()));
    }
}
