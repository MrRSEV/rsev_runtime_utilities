using System.Collections.ObjectModel;

namespace RSEV.Utilities.Plugins;

/// <summary>Shared context passed to framework extension plugins.</summary>
public sealed class GeneratorContext
{
    public GeneratorContext(IDictionary<string, object?>? values = null) => Values = new ReadOnlyDictionary<string, object?>(values ?? new Dictionary<string, object?>());
    public IReadOnlyDictionary<string, object?> Values { get; }
}
