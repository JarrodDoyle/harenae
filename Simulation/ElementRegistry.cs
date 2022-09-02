namespace Simulation;

public static class ElementRegistry
{
    private static Dictionary<string, Element> _elements = new ();

    public static bool RegisterElement(string name, ElementSettings settings)
    {
        return _elements.TryAdd(name, new Element(name, settings));
    }

    public static bool DeregisterElement(string name) {
        return _elements.Remove(name);
    }

    public static void DeregisterAllElements()
    {
        _elements.Clear();
    }

    public static Element GetElement(string name)
    {
        return _elements[name];
    }

    public static string[] GetElementNames()
    {
        return _elements.Keys.ToArray();
    }
}