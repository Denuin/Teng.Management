using System.Management;
using Teng.Management;

var usbWatcher = new USB();
usbWatcher.AddUSBEventWatcher(USBEventHandler, USBEventHandler, new TimeSpan(0, 0, 1));

Console.ReadLine();

usbWatcher.RemoveUSBEventWatcher();

static void USBEventHandler(object sender, EventArrivedEventArgs e)
{
#if NET5_0_OR_GREATER
    if (!OperatingSystem.IsWindows()) return;
#endif

    var watcher = sender as ManagementEventWatcher;
    watcher.Stop();

    var drive = new Drive();
    var lsta = Drive.GetLogicalDiskDeviceID();

    var lstb = e.USBControllerDevice();

    Console.WriteLine($"LogicalDiskDeviceID: {string.Join(",", lsta)}");

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