using System;
using System.Management;

namespace Teng.Management
{
    /// <summary>
    /// USB控制设备类型
    /// </summary>
    public struct USBControllerDevice
    {
        /// <summary>
        /// USB控制器设备ID
        /// </summary>
        public string Antecedent;

        /// <summary>
        /// USB即插即用设备ID
        /// </summary>
        public string Dependent;
    }

    public class USB
    {
        private ManagementEventWatcher insertWatcher = null;
        private ManagementEventWatcher removeWatcher = null;

        public bool AddUSBEventWatcher(EventArrivedEventHandler usbInsertHandler, EventArrivedEventHandler usbRemoveHandler, TimeSpan withinInterval)
        {
#if NET5_0_OR_GREATER
            if (!OperatingSystem.IsWindows()) return false;
#endif
            try
            {
                var scope = new ManagementScope("root\\CIMV2");
                scope.Options.EnablePrivileges = true;

                // USB插入监视
                if (usbInsertHandler != null)
                {
                    var insertQuery = new WqlEventQuery("__InstanceCreationEvent",
                        withinInterval,
                        "TargetInstance isa 'Win32_USBControllerDevice'");
                    insertWatcher = new ManagementEventWatcher(scope, insertQuery);
                    insertWatcher.EventArrived += usbInsertHandler;
                    insertWatcher.Start();
                }

                // USB拔出监视
                if (usbRemoveHandler != null)
                {
                    var removeQuery = new WqlEventQuery("__InstanceDeletionEvent",
                        withinInterval,
                        "TargetInstance isa 'Win32_USBControllerDevice'");
                    removeWatcher = new ManagementEventWatcher(scope, removeQuery);
                    removeWatcher.EventArrived += usbRemoveHandler;
                    removeWatcher.Start();
                }

                return true;
            }
            catch
            {
                RemoveUSBEventWatcher();
                return false;
            }
        }

        /// <summary>
        /// 移去USB事件监视器
        /// </summary>
        public void RemoveUSBEventWatcher()
        {
#if NET5_0_OR_GREATER
            if (!OperatingSystem.IsWindows()) return;
#endif
            if (insertWatcher != null)
            {
                insertWatcher.Stop();
                insertWatcher = null;
            }
            if (removeWatcher != null)
            {
                removeWatcher.Stop();
                removeWatcher = null;
            }
        }
    }

    public static class USBExtentions
    {
        /// <summary>
        /// 定位发生插拔的USB设备
        /// </summary>
        /// <param name="e">USB拔插事件参数</param>
        /// <returns>发生拔插现象的USB控制设备ID</returns>
        public static USBControllerDevice[] USBControllerDevice(this EventArrivedEventArgs e)
        {
#if NET5_0_OR_GREATER
            if (!OperatingSystem.IsWindows()) return null;
#endif
            var mbo = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            if (mbo != null && mbo.ClassPath.ClassName == "Win32_USBControllerDevice")
            {
                string antecedent = mbo["Antecedent"].ToString().Replace("\"", string.Empty).Split('=')[1];
                string dependent = mbo["Dependent"].ToString().Replace("\"", string.Empty).Split('=')[1];
                return new USBControllerDevice[] {
                    new() {
                        Antecedent = antecedent,
                        Dependent = dependent
                    }
                };
            }
            return null;
        }

        /// <summary>
        /// USB插入事件
        /// </summary>
        public static bool IsCreationEvent(this EventArrivedEventArgs e)
        {
#if NET5_0_OR_GREATER
            if (!OperatingSystem.IsWindows()) return false;
#endif
            return (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent");
        }

        /// <summary>
        /// USB拔出事件
        /// </summary>
        public static bool IsDeletionEvent(this EventArrivedEventArgs e)
        {
#if NET5_0_OR_GREATER
            if (!OperatingSystem.IsWindows()) return false;
#endif
            return (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent");
        }
    }
}