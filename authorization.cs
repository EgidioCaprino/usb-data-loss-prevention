public class AuthorizationService {
  private static readonly String AUTHORIZATION_FILE = @"C:\Windows\authorized_usb_devices.txt";

  private Logger logger = new Logger();
  private HashSet<String> authorizedDevices;

  public AuthorizationService() {
    authorizedDevices = new HashSet<String>();
    if (File.Exists(AUTHORIZATION_FILE)) {
      logger.Information($"Reading authorized devices from {AUTHORIZATION_FILE}...");
      String[] lines = File.ReadAllLines(AUTHORIZATION_FILE);
      foreach (var line in lines) {
        String trimmedLine = line.Trim();
        if (trimmedLine.Length > 0) {
          authorizedDevices.Add(trimmedLine);
        }
      }
      logger.Information($"Found {authorizedDevices.Count} authorized devices");
    } else {
      logger.Error($"File {AUTHORIZATION_FILE} not found: all devices will be unauthorized");
    }
  }

  public Boolean IsAuthorized(String deviceID) {
    return authorizedDevices.Contains(deviceID);
  }
}
