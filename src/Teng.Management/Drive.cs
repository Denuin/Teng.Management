using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace Teng.Management
{
    /// <summary>
    /// 驱动器
    /// </summary>
    public class Drive
    {
        /// <summary>
        /// 获取盘符
        /// </summary>
        public static IEnumerable<string> GetLogicalDiskDeviceID()
        {
            var q = new Queue<string>();
#if NET5_0_OR_GREATER
            if (!OperatingSystem.IsWindows()) return null;
#endif
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
            ManagementObjectCollection query = searcher.Get();
            foreach (ManagementObject mo in query.Cast<ManagementObject>())
            {
                var driveType = int.Parse(mo["DriveType"].ToString());
                if (driveType == (int)DriveType.Removable || driveType == (int)DriveType.Fixed)
                {
                    q.Enqueue(mo["DeviceID"].ToString());
                }
            }
            return q.ToArray();
        }
    }
}