using System.Diagnostics;

public class DiskLogger {
  private Logger logger = new Logger();
  private String driveLetter;

  public DiskLogger(String driveLetter) {
    this.driveLetter = driveLetter;
  }

  public void Information(String message) {
    logger.Information($"[Disk {driveLetter}] {message}");
  }
}

public class Logger {
  private EventLog eventLog;

  public Logger() {
    eventLog = new EventLog();
    eventLog.Source = "USB Data Loss Prevention";
  }

  public void Information(String message) {
    eventLog.WriteEntry(message, EventLogEntryType.Information);
  }

  public void Warning(String message) {
    eventLog.WriteEntry(message, EventLogEntryType.Warning);
  }

  public void Error(String message) {
    eventLog.WriteEntry(message, EventLogEntryType.Error);
  }
}
