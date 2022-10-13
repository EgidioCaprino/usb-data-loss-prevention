using System.Management;

EventService eventService = new EventService();
DiskService diskService = new DiskService();

eventService.OnLogicalDisk((object sender, EventArrivedEventArgs arguments) => {
  ManagementBaseObject targetInstance = (ManagementBaseObject) arguments.NewEvent.GetPropertyValue("TargetInstance");
  Disk disk = new Disk(targetInstance);
  Console.WriteLine("Device ID: {0}", disk.GetDeviceID());
  Boolean readOnly = diskService.IsReadOnly(disk);
  Console.WriteLine("Read only: {0}", readOnly);
});

for (;;);
