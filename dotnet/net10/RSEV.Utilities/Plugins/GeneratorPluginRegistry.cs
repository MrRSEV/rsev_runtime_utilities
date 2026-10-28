namespace RSEV.Utilities.Plugins;

public sealed class GeneratorPluginRegistry
{
    private readonly Dictionary<string, IGeneratorPlugin> _plugins = new(StringComparer.Ordinal);
    public void Register(IGeneratorPlugin plugin)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        if (!_plugins.TryAdd(plugin.Name, plugin)) throw new InvalidOperationException($"Plugin '{plugin.Name}' is already registered.");
    }
    public IReadOnlyList<IGeneratorPlugin> All() => _plugins.Values.OrderBy(plugin => plugin.Name, StringComparer.Ordinal).ToArray();
    public void Generate(GeneratorContext context) { foreach (var plugin in All()) plugin.Generate(context); }
}
