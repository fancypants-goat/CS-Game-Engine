using System.Reflection;

namespace Engine;

public struct Parameter(object value, Type type)
{
    public readonly object Value = value;
    public readonly Type Type = type;
}

internal static class ComponentRegistry
{
    readonly private static Dictionary<string, Type> components = [];

    static ComponentRegistry()
    {
        var assembly = typeof(Component).Assembly;

        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(Component).IsAssignableFrom(type)) // if a variable of type Component can be assigned to of a type 'type'
                continue;
            
            var meta = type.GetCustomAttribute<ComponentMetaAttribute>(); // get the ComponentMeta data assigned to the type
            if (meta != null)
            {
                components[meta.Name] = type;
            }
        }
    }
    
    public static bool GetComponentType(string componentName, out Type? type)
        => components.TryGetValue(componentName, out type);

    public static Component Create(Type type, List<Parameter> arguments)
    {
        foreach (var ctor in type.GetConstructors())
        {
            var parameters = ctor.GetParameters();
            
            // check if all parameters match
            if (parameters.Length != arguments.Count) continue;

            var match = !parameters.Where((t, i) => arguments[i].Type != t.ParameterType).Any();
            if (!match) continue;

            var args = arguments.Select(arg => arg.Value).ToArray();
            return (Component)ctor.Invoke(args);
        }
        
        throw new Exception($"No matching constructor found for {type.Name}");
    }
}