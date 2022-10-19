using System.Management;

EventService eventService = new EventService();
DiskService diskService = new DiskService();
AuthorizationService authorizationService = new AuthorizationService();
Logger logger = new Logger();

logger.Information("Started");

eventService.OnVolume((object sender, EventArrivedEventArgs arguments) => {
  try {
    logger.Information("Disk connected");
    ManagementBaseObject targetInstance = (ManagementBaseObject) arguments.NewEvent.GetPropertyValue("TargetInstance");
    Disk disk = new Disk(targetInstance);
    DiskLogger diskLogger = new DiskLogger(disk.GetDriveLetter());
    diskLogger.Information(disk.ToString());
    diskLogger.Information($"Device ID: {disk.GetDeviceID()}");
    Boolean authorized = authorizationService.IsAuthorized(disk.GetDeviceID());
    diskLogger.Information($"Authorized: {authorized}");
    if (!authorized) {
      Boolean readOnly = diskService.IsReadOnly(disk);
      diskLogger.Information($"Read only: {readOnly}");
      if (!readOnly) {
        diskLogger.Information($"Setting drive {disk.GetDriveLetter()} in read-only mode...");
        diskService.SetReadOnly(disk, true);
        diskLogger.Information($"Read only: {readOnly}");
      }
    }
  } catch (Exception exception) {
    logger.Error($"An error occurred: {exception.Message}\n{exception.ToString()}");
  }
});

logger.Information("Listening for new devices...");

for (;;);
