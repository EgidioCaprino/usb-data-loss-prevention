using System.Management;

EventService eventService = new EventService();
eventService.OnLogicalDisk((object sender, EventArrivedEventArgs arguments) => {
  ManagementBaseObject targetInstance = (ManagementBaseObject) arguments.NewEvent.GetPropertyValue("TargetInstance");
  Disk drive = new Disk(targetInstance);
  Console.WriteLine("Device ID: {0}", drive.GetDeviceID());
});

for (;;);
