namespace Engine;

[AttributeUsage(AttributeTargets.Class)]
public class ComponentMetaAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}