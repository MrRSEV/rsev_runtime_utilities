namespace RSEV.Utilities.Plugins;

public interface IGeneratorPlugin
{
    string Name { get; }
    Version Version { get; }
    void Generate(GeneratorContext context);
}
