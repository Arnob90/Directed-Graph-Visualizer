using Godot;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public static partial class CurrentDefaultSerializationOptions
{
    public static JsonSerializerOptions DefaultOption = new();
}
