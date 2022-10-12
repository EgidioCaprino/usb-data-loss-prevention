using System.Management;

public class EventService {
  public void OnLogicalDisk(EventArrivedEventHandler handler) {
    ManagementScope managementScope = new ManagementScope("root\\CIMV2");
    managementScope.Options.EnablePrivileges = true;
    
    WqlEventQuery eventQuery = new WqlEventQuery();
    eventQuery.EventClassName = "__InstanceCreationEvent";
    eventQuery.WithinInterval = new TimeSpan(0, 0, 3);
    eventQuery.Condition = @"TargetInstance ISA 'Win32_LogicalDisk'";

    ManagementEventWatcher eventWatcher = new ManagementEventWatcher(managementScope, eventQuery);
    eventWatcher.EventArrived += new EventArrivedEventHandler(handler);
    eventWatcher.Start();
  }
}
