using System.Management;

EventService eventService = new EventService();
DiskService diskService = new DiskService();
AuthorizationService authorizationService = new AuthorizationService();

eventService.OnVolume((object sender, EventArrivedEventArgs arguments) => {
  ManagementBaseObject targetInstance = (ManagementBaseObject) arguments.NewEvent.GetPropertyValue("TargetInstance");
  Disk disk = new Disk(targetInstance);
  Console.WriteLine("Disk Connected");
  Console.WriteLine(disk.ToString());
  Console.WriteLine("Drive Letter: {0}", disk.GetDriveLetter());
  Console.WriteLine("Device ID: {0}", disk.GetDeviceID());
  Boolean authorized = authorizationService.IsAuthorized(disk.GetDeviceID());
  Console.WriteLine("Authorized: {0}", authorized);
  if (!authorized) {
    Boolean readOnly = diskService.IsReadOnly(disk);
    Console.WriteLine("Read only: {0}", readOnly);
    if (!readOnly) {
      Console.WriteLine("Setting drive {0} in read-only mode", disk.GetDriveLetter());
      diskService.SetReadOnly(disk, true);
      Console.WriteLine("Read only: {0}", diskService.IsReadOnly(disk));
    }
  }
});

for (;;);
