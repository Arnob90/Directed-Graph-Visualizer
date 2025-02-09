using System;
using System.Reflection;
namespace UltraUnitTesterSpace; 
[AttributeUsage(AttributeTargets.Class)]
class TestClassAttribute:Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
class TestMethodAttribute:Attribute
{
}
