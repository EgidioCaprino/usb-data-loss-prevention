using System.Management;

EventService eventService = new EventService();
DiskService diskService = new DiskService();

eventService.OnVolume((object sender, EventArrivedEventArgs arguments) => {
  ManagementBaseObject targetInstance = (ManagementBaseObject) arguments.NewEvent.GetPropertyValue("TargetInstance");
  Disk disk = new Disk(targetInstance);
  Console.WriteLine("Disk Connected");
  Console.WriteLine(disk.ToString());
  Console.WriteLine("Drive Letter: {0}", disk.GetDriveLetter());
  Console.WriteLine("Device ID: {0}", disk.GetDeviceID());
  Boolean readOnly = diskService.IsReadOnly(disk);
  Console.WriteLine("Read only: {0}", readOnly);
  if (!readOnly) {
    diskService.SetReadOnly(disk, true);
    Console.WriteLine("Read only: {0}", diskService.IsReadOnly(disk));
  }
});

for (;;);
