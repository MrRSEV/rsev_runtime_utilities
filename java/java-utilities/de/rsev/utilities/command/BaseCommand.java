package de.rsev.utilities.command;

/**
 * <summary>
 * Abstrakte Basisklasse für alle Commands.
 * Stellt einen einheitlichen Namens- und Alias-Mechanismus bereit
 * und dient als Fundament für erweiterbare Command-Logik.
 * </summary>
 */
public abstract class BaseCommand implements ICommand {

    protected final String name;
    protected final String[] aliases;

    /**
     * <summary>
     * Erstellt einen neuen Command mit Namen und optionalen Aliases.
     * </summary>
     */
    protected BaseCommand(String name, String... aliases) {
        this.name = name;
        this.aliases = aliases;
    }

    /* -----------------------------
       Accessor Methods
       ----------------------------- */

    /**
     * <summary>
     * Gibt den primären Namen des Commands zurück.
     * </summary>
     */
    @Override
    public String getName() {
        return name;
    }

    /**
     * <summary>
     * Gibt alternative Namen (Aliases) für den Command zurück.
     * </summary>
     */
    @Override
    public String[] getAliases() {
        return aliases;
    }
}
