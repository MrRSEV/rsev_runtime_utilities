package de.rsev.utilities.logging;

public class LoggerAdapter implements ILogger {

    @Override
    public void debug(String message) {
        BaseLogger baseLogger = new BaseLogger();
        baseLogger.debug(message);
    }

    @Override
    public void info(String message) {
        BaseLogger baseLogger = new BaseLogger();
        baseLogger.info(message);
    }

    @Override
    public void warn(String message) {
        BaseLogger baseLogger = new BaseLogger();
        baseLogger.warn(message);
    }

    public void error(String message) {
        BaseLogger baseLogger = new BaseLogger();
        baseLogger.error(message);
    }

    @Override
    public void error(String message, Throwable throwable) {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'error'");
    }

    @Override
    public void critical(String message) {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'critical'");
    }
}
