package de.rsev.utilities.communication.smtp;

import java.io.*;
import java.net.Socket;
import java.nio.charset.StandardCharsets;

/**
 * <summary>
 * Kapselt die TCP-Verbindung zu einem SMTP-Server
 * inklusive Lesen und Schreiben von SMTP-Kommandos.
 * </summary>
 */
public class SmtpConnection implements Closeable {

    protected Socket socket;
    protected BufferedReader reader;
    protected BufferedWriter writer;

    public SmtpConnection(String host, int port) throws IOException {
        this.socket = new Socket(host, port);
        this.reader = new BufferedReader(
                new InputStreamReader(socket.getInputStream(), StandardCharsets.UTF_8)
        );
        this.writer = new BufferedWriter(
                new OutputStreamWriter(socket.getOutputStream(), StandardCharsets.UTF_8)
        );
    }

    /* -----------------------------
       IO Methods
       ----------------------------- */

    public void sendLine(String line) throws IOException {
        writer.write(line);
        writer.write("\r\n");
        writer.flush();
    }

    public SmtpResponse readResponse() throws IOException {
        String line = reader.readLine();
        if (line == null || line.length() < 3)
            throw new IOException("Ungültige SMTP-Antwort");

        int code = Integer.parseInt(line.substring(0, 3));
        String msg = line.length() > 4 ? line.substring(4) : "";

        return new SmtpResponse(code, msg);
    }

    @Override
    public void close() throws IOException {
        socket.close();
    }

    protected void replaceSocket(Socket newSocket) throws IOException {
        this.socket = newSocket;

        this.reader = new BufferedReader(
                new InputStreamReader(newSocket.getInputStream())
        );
        this.writer = new BufferedWriter(
                new OutputStreamWriter(newSocket.getOutputStream())
        );
    }


}
