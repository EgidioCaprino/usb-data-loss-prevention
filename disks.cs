using System.Management;

public class Disk {
  private ManagementBaseObject baseObject;
  private String deviceID;

  public Disk(ManagementBaseObject baseObject) {
    this.baseObject = baseObject;
  }

  public String GetDeviceID() {
    if (deviceID == null) {
      foreach (var property in baseObject.Properties) {
        if ("DeviceID".Equals(property.Name)) {
          deviceID = (String) property.Value;
          break;
        }
      }
      if (deviceID == null) {
        throw new Exception("Device ID property not found in ManagementBaseObject");
      }
    }
    return deviceID;
  }
}
