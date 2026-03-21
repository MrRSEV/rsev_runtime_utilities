package de.rsev.utilities.controller;

/**
 * <summary>
 * Abstrakte Basisklasse für Controller-Komponenten.
 * Implementiert einen einfachen Aktivitäts-Lifecycle
 * mit Initialisierungs- und Shutdown-Phase.
 * </summary>
 */
public abstract class BaseController implements IController {

    protected boolean active;

    /* -----------------------------
       Lifecycle Methods
       ----------------------------- */

    /**
     * <summary>
     * Initialisiert den Controller und markiert ihn als aktiv.
     * </summary>
     */
    @Override
    public void initialize() {
        active = true;
    }

    /**
     * <summary>
     * Beendet den Controller und markiert ihn als inaktiv.
     * </summary>
     */
    @Override
    public void shutdown() {
        active = false;
    }
}
