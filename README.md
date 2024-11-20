# Teng.Management

Get hardware and system information

## How to use

### 1. USB
```cs
using Teng.Management;

// USB watcher
var usbWatcher = new USB();
usbWatcher.AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 1));

Console.ReadLine();

usbWatcher.RemoveUSBEventWatcher();
```
```cs
static void USBEventHandler(object sender, EventArrivedEventArgs e)
{
    var watcher = sender as ManagementEventWatcher;
    watcher.Stop();

    if (e.IsCreationEvent())
    {
        Console.WriteLine("USBDeviceTips.NewUSBDevice");
    }
    else if (e.IsDeletionEvent())
    {
        Console.WriteLine("USBDeviceTips.USBDeviceDisconnected");
    }
	
    watcher.Start();
}

```

### 2. Drive
```cs
using Teng.Management;

var drive = new Drive();
// Get deviceId of all drives
var lsta = drive.GetLogicalDiskDeviceID();

Console.WriteLine($"LogicalDiskDeviceID: {string.Join(",", lsta)}");

```

## Change log

 - 1.0.0 (2024-11-20)
	- First release

> https://github.com/Denuin/Teng.Management
> https://www.nuget.org/packages/Teng.Management