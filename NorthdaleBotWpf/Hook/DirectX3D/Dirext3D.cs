// NOT WRITTEN BY ME - ALL RIGHTS TO RESPECTIVE OWNER
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthdaleBotWpf.Hook.DirectX
{
    internal class Dirext3D // not mine
    {
        public Dirext3D(Process targetProc)
        {
            TargetProcess = targetProc;

            UsingDirectX11 = TargetProcess.Modules.Cast<ProcessModule>().Any(m => m.ModuleName == "d3d11.dll");

            using (var d3D = UsingDirectX11
                                       ? (D3DDevice)new D3D11Device(targetProc)
                                       : new D3D9Device(targetProc))
            {
                HookPtr = UsingDirectX11 ? ((D3D11Device)d3D).GetSwapVTableFuncAbsoluteAddress(d3D.PresentVtableIndex) : d3D.GetDeviceVTableFuncAbsoluteAddress(d3D.EndSceneVtableIndex);
            }
        }

        public Process TargetProcess { get; private set; }
        public bool UsingDirectX11 { get; private set; }
        public IntPtr HookPtr { get; private set; }
    }
}
