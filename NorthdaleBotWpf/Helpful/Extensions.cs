using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NorthdaleBotWpf.Helpful
{
    static class Extensions
    {
        #region Process

        public static IntPtr BaseAddress(this Process proc)
        {
            return proc.MainModule.BaseAddress;
        }

        #endregion
    }
}
