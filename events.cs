using System.Management;

public class EventService {
  public void OnVolume(EventArrivedEventHandler handler) {
    ManagementScope managementScope = new ManagementScope("root\\CIMV2");
    managementScope.Options.EnablePrivileges = true;
    
    WqlEventQuery eventQuery = new WqlEventQuery();
    eventQuery.EventClassName = "__InstanceCreationEvent";
    eventQuery.WithinInterval = new TimeSpan(0, 0, 3);
    eventQuery.Condition = @"TargetInstance ISA 'Win32_Volume'";

    ManagementEventWatcher eventWatcher = new ManagementEventWatcher(managementScope, eventQuery);
    eventWatcher.EventArrived += new EventArrivedEventHandler(handler);
    eventWatcher.Start();
  }
}
