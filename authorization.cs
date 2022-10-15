public class AuthorizationService {
  private HashSet<String> authorizedDevices;

  public AuthorizationService() {
    String[] lines = File.ReadAllLines(@"C:\Windows\authorized_usb_devices.txt");
    authorizedDevices = new HashSet<String>();
    foreach (var line in lines) {
      String trimmedLine = line.Trim();
      if (trimmedLine.Length > 0) {
        authorizedDevices.Add(trimmedLine);
      }
    }
  }

  public Boolean IsAuthorized(String deviceID) {
    return authorizedDevices.Contains(deviceID);
  }
}
