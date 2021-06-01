// NOT WRITTEN BY ME - ALL RIGHTS TO RESPECTIVE OWNER
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthdaleBotWpf.Memory
{
    public static class Utility
    {
        public static readonly Random Rand = new Random();

        // returns base offset for main module
        public static uint BaseOffset(this Process proc)
        {
            return (uint)proc.MainModule.BaseAddress.ToInt32();
        }

        public static string VersionString(this Process proc)
        {
            return proc.MainModule.FileVersionInfo.FileVersion;
        }
    }
}
