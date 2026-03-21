package de.rsev.utilities.command;

public interface ICommand {
    
    /** 
     * <summary>
     *  Der eindeutige Name des Commands.
     * </summary> 
     */
    String getName();
    
    /** 
     * <summary>
     *  Optional
     *  Mögliche definierbare Alias Namen
     *  Hilft gegen unbeabsichtigte Tippfehler
     * </summary> 
     */
    default String[] getAliases() {
        return new String[0];
    }
    
    /** 
     * <summary>
     *  Führt den Command aus.
     *  <param name="args">Optionale Argument Parameter für den Command.</param>
     * </summary> 
     */
    void execute(String[] args);
}

