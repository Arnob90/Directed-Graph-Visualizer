using Godot;
using Optional;
using System;
using System.Collections.Generic;
namespace MiscSpace;
public partial class Misc
{
    public static Dictionary<T2, T1> GetInverseDict<T1, T2>(IDictionary<T1, T2> givenDict)
    {
        var requiredInverseMap = new Dictionary<T2, T1>();
        foreach (var (key, val) in givenDict)
        {
            if (!requiredInverseMap.TryAdd(val, key))
            {
                throw new ArgumentException("The given dict is not bijective");
            }
        }
        return requiredInverseMap;
    }
}
public static class OptionExtensions
{
    public static Option<T> FromNullable<T>(this T nullable) where T:class
    {
        return nullable == null ? Option.None<T>() : Option.Some(nullable);
    }
    public static Option<T> FromNullable<T>(this T? nullable) where T:struct
    {
        return nullable == null ? Option.None<T>() : Option.Some((T)nullable);
    }
}
