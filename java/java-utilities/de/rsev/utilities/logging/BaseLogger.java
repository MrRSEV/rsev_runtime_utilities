package de.rsev.utilities.logging;

import java.io.BufferedWriter;
import java.io.IOException;
import java.nio.file.*;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

/**
 * <summary>
 * Zentrales, systemnahes Logging-System.
 * Unterstützt farbige Konsolenausgabe, Log-Level,
 * Log-Rotation und OS-abhängige Log-Verzeichnisse.
 * </summary>
 */
public class BaseLogger implements ILogger {

    private static BufferedWriter logWriter;
    private static Path logDirectory;
    private static Path logFilePath;

    private static final String LOG_FILE_NAME = "latest.log";
    private static final String[] LOG_LEVEL_NAMES =
            { "INFO", "WARNING", "ERROR", "CRITICAL", "DEBUG" };

    private static boolean closed = false;

    /* =====================================================
       Enums
       ===================================================== */

    public enum LogColor {
        Blue, Green, Cyan, Red, Magenta, Yellow, White, Gray,
        DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta,
        DarkYellow, DarkGray
    }

    public enum LogLevel {
        INFO(0),
        WARNING(1),
        ERROR(2),
        CRITICAL(3),
        DEBUG(4);

        private final int level;

        LogLevel(int level) {
            this.level = level;
        }

        public int value() {
            return level;
        }
    }

    /* =====================================================
       Init / Shutdown
       ===================================================== */

    public static synchronized void init() {
        try {
            String os = System.getProperty("os.name").toLowerCase();
            logDirectory = os.contains("win")
                    ? Paths.get(System.getProperty("user.dir"), "log")
                    : Paths.get("/var/log/rsev-utilities");

            Files.createDirectories(logDirectory);
            logFilePath = logDirectory.resolve(LOG_FILE_NAME);

            rotateIfExists();

            logWriter = Files.newBufferedWriter(
                    logFilePath,
                    StandardOpenOption.CREATE,
                    StandardOpenOption.WRITE
            );

        } catch (IOException ex) {
            System.err.println("[CRITICAL] Logger init failed: " + ex.getMessage());
        }
    }

    public static synchronized void close() {
        if (closed) return;
        closed = true;

        try {
            if (logWriter != null)
                logWriter.close();

            rotateIfExists();
        } catch (IOException ex) {
            System.err.println("[WARNING] Could not archive log file: " + ex.getMessage());
        }
    }

    private static void rotateIfExists() throws IOException {
        if (Files.exists(logFilePath)) {
            String ts = LocalDateTime.now()
                    .format(DateTimeFormatter.ofPattern("yyyy-MM-dd-HH-mm-ss"));
            Files.move(
                    logFilePath,
                    logDirectory.resolve(ts + ".log"),
                    StandardCopyOption.REPLACE_EXISTING
            );
        }
    }

    /* =====================================================
       Core logging
       ===================================================== */

    public static void log(LogLevel level, LogColor color, String message) {
        if (closed) return;

        String ts = LocalDateTime.now()
                .format(DateTimeFormatter.ofPattern("dd-MM HH:mm:ss"));

        String formatted =
                "[" + ts + "] - [" + LOG_LEVEL_NAMES[level.value()] + "] -> " + message;

        String ansi = color != null
                ? toAnsiColor(color)
                : defaultColor(level);

        if (ansi != null)
            System.out.println(ansi + formatted + ANSI_RESET);
        else
            System.out.println(formatted);

        try {
            if (logWriter != null) {
                logWriter.write(formatted);
                logWriter.newLine();
                logWriter.flush();
            }
        } catch (IOException ignored) {}
    }

    /* =========================
       STATIC SYSTEM LOGGING
       ========================= */

    public void debug(String message) {
        log(LogLevel.DEBUG, null, message);
    }

    public void info(String message) {
        log(LogLevel.INFO, null, message);
    }

    public void warn(String message) {
        log(LogLevel.WARNING, null, message);
    }

    public void error(String message) {
        log(LogLevel.ERROR, null, message);
    }

    @Override
    public void error(String message, Throwable throwable) {
        log(LogLevel.ERROR, null, message);
    }

    @Override
    public void critical(String message) {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'critical'");
    }

    /* =====================================================
       ANSI helpers
       ===================================================== */

    private static final String ANSI_RESET = "\u001B[0m";

    private static String defaultColor(LogLevel level) {
        return switch (level) {
            case WARNING -> "\u001B[33m";
            case ERROR -> "\u001B[35m";
            case CRITICAL -> "\u001B[31m";
            case DEBUG -> "\u001B[32m";
            default -> null;
        };
    }

    private static String toAnsiColor(LogColor color) {
        return switch (color) {
            case Gray -> "\u001B[37m";
            case Green -> "\u001B[32m";
            case Yellow -> "\u001B[33m";
            case Red -> "\u001B[31m";
            case Cyan -> "\u001B[36m";
            case Magenta -> "\u001B[35m";
            case White -> "\u001B[97m";
            case Blue -> "\u001B[34m";
            case DarkGray -> "\u001B[90m";
            case DarkGreen -> "\u001B[32m";
            case DarkYellow -> "\u001B[33m";
            case DarkRed -> "\u001B[31m";
            case DarkCyan -> "\u001B[36m";
            case DarkMagenta -> "\u001B[35m";
            default -> "\u001B[37m";
        };
    }

}
