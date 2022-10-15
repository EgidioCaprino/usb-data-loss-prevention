using System.Diagnostics;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;

public class Disk {
  private static Regex driveLetterRegex = new Regex("([A-Z]):");
  private static Regex deviceIDRegex = new Regex("\\{([A-Za-z0-9-]+)\\}");

  private ManagementBaseObject baseObject;
  private String driveLetter;
  private String deviceID;

  public Disk(ManagementBaseObject baseObject) {
    this.baseObject = baseObject;
  }

  public String GetDriveLetter() {
    if (driveLetter == null) {
      foreach (var property in baseObject.Properties) {
        if ("DriveLetter".Equals(property.Name)) {
          driveLetter = (String) property.Value;
          break;
        }
      }
      if (driveLetter == null) {
        throw new Exception("DriveLetter property not found in ManagementBaseObject");
      }
      Match match = driveLetterRegex.Match(driveLetter);
      if (match.Success) {
        driveLetter = match.Groups[1].Value;
      } else {
        Console.WriteLine("Suspected wrong drive letter value: {0}", driveLetter);
      }
    }
    return driveLetter;
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
        throw new Exception("DeviceID property not found in ManagementBaseObject");
      }
      Match match = deviceIDRegex.Match(deviceID);
      if (match.Success) {
        deviceID = match.Groups[1].Value;
      } else {
        Console.WriteLine("Suspected wrong device ID value: {0}", deviceID);
      }
    }
    return deviceID;
  }

  public String ToString() {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (var property in baseObject.Properties) {
      stringBuilder.AppendFormat("{0}: {1}", property.Name, property.Value);
      stringBuilder.AppendLine();
    }
    return stringBuilder.ToString();
  }
}

public class DiskService {
  private static Regex selectedVolumeRegex = new Regex("Volume [0-9]+ is the selected volume");
  private static Regex readOnlyStateRegex = new Regex("Current Read-only State : ([A-Z][a-z]{1,2})");
  private static Regex attributesClearedRegex = new Regex("Disk attributes cleared successfully");
  private static Regex attributesSetRegex = new Regex("Disk attributes set successfully");

  public Boolean IsReadOnly(Disk disk) {
    Process process = startDiskPart();
    try {
      selectVolume(process, disk.GetDriveLetter());
      String output = execute(process, "attributes disk", readOnlyStateRegex);
      Match match = readOnlyStateRegex.Match(output);
      String answer = match.Groups[1].Value;
      if ("Yes".Equals(answer)) {
        return true;
      } else if ("No".Equals(answer)) {
        return false;
      } else {
        throw new Exception($"Invalid output: {output}");
      }
    } finally {
      exitDiskPart(process);
    }
  }

  public void SetReadOnly(Disk disk, Boolean readOnly) {
    Process process = startDiskPart();
    try {
      selectVolume(process, disk.GetDriveLetter());
      execute(process, "attributes disk clear readonly", attributesClearedRegex);
      if (readOnly) {
        execute(process, "attributes disk set readonly", attributesSetRegex);
      }
    } finally {
      exitDiskPart(process);
    }
  }

  private Process startDiskPart() {
    Process process = new Process();
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.FileName = @"C:\Windows\System32\diskpart.exe";
    process.StartInfo.RedirectStandardInput = true;
    process.Exited += (object? sender, EventArgs arguments) => {
      Console.WriteLine("diskpart exited");
    };
    process.Start();
    process.BeginOutputReadLine();
    return process;
  }

  private void exitDiskPart(Process process) {
    process.StandardInput.WriteLine("exit");
    process.WaitForExit();
  }

  private void selectVolume(Process process, String driveLetter) {
    execute(process, $"select volume {driveLetter}", selectedVolumeRegex);
  }

  private String execute(Process process, String command, Regex target) {
    Console.WriteLine($"Executing diskpart command: {command}");
    StringBuilder outputBuilder = new StringBuilder();
    Boolean matches = false;
    process.OutputDataReceived += (object sender, DataReceivedEventArgs arguments) => {
      String line = arguments.Data;
      outputBuilder.AppendLine(line);
      if (!matches) {
        matches = target.IsMatch(line);
      }
    };
    try {
      process.StandardInput.WriteLine(command);
      DateTime startTime = DateTime.Now;
      while (!matches) {
        if (DateTime.Now.Subtract(startTime).TotalMinutes > 1) {
          Console.WriteLine($"Output: {outputBuilder.ToString()}");
          throw new Exception($"diskpart command \"{command}\" is pending for more than one minute");
        }
        Thread.Sleep(100);
      }
      String output = outputBuilder.ToString();
      Console.WriteLine($"Output: {output}");
      return output;
    } finally {
      process.OutputDataReceived += null;
    }
  }
}
