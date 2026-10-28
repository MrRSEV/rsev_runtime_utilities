using System.Reflection;

namespace RSEV.Utilities.Plugins;

public sealed class GeneratorPluginLoader
{
    public int Discover(string directory, GeneratorPluginRegistry registry)
    {
        if (!Directory.Exists(directory)) return 0;
        var count = 0;
        foreach (var path in Directory.EnumerateFiles(directory, "*.dll", SearchOption.TopDirectoryOnly).OrderBy(value => value, StringComparer.Ordinal))
        {
            var assembly = Assembly.LoadFrom(path);
            foreach (var type in assembly.GetTypes().Where(type => typeof(IGeneratorPlugin).IsAssignableFrom(type) && type is { IsAbstract: false, IsInterface: false }))
            {
                if (Activator.CreateInstance(type) is IGeneratorPlugin plugin) { registry.Register(plugin); count++; }
            }
        }
        return count;
    }
}
