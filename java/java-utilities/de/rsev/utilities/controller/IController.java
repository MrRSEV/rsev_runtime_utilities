package de.rsev.utilities.controller;

/**
 * <summary>
 * Definiert den grundlegenden Lifecycle für Controller-Komponenten.
 * Controller kapseln logische Steuerungs- oder Koordinationsaufgaben
 * innerhalb der Runtime.
 * </summary>
 */
public interface IController {

    /**
     * <summary>
     * Initialisiert den Controller.
     * </summary>
     */
    void initialize();

    /**
     * <summary>
     * Fährt den Controller kontrolliert herunter.
     * </summary>
     */
    void shutdown();
}
