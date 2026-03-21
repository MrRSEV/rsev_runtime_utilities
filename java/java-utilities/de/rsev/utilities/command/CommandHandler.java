package de.rsev.utilities.command;

import java.util.*;

/**
 * <summary>
 * Zentrale Registry und Dispatcher-Klasse für Commands.
 * Verantwortlich für Registrierung, Lookup und Ausführung
 * von Command-Instanzen anhand von Benutzereingaben.
 * </summary>
 */
public class CommandHandler {

    private final Map<String, ICommand> commands = new HashMap<>();

    /* -----------------------------
       Registration Methods
       ----------------------------- */

    /**
     * <summary>
     * Registriert einen Command unter seinem primären Namen.
     * </summary>
     */
    public void register(ICommand command) {
        commands.put(command.getName(), command);
    }

    /* -----------------------------
       Execution Methods
       ----------------------------- */

    /**
     * <summary>
     * Parst eine Eingabezeile, ermittelt den passenden Command
     * und führt diesen mit den übergebenen Argumenten aus.
     * </summary>
     */
    public void handle(String input) {
        String[] split = input.split(" ");
        ICommand cmd = commands.get(split[0]);

        if (cmd != null) {
            cmd.execute(Arrays.copyOfRange(split, 1, split.length));
        }
    }
}
