﻿// NOT WRITTEN BY ME - ALL RIGHTS TO RESPECTIVE OWNER
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthdaleBotWpf.Hook.DirectX
{
    internal abstract class D3DDevice : IDisposable // not mine
    {
        protected readonly Process TargetProcess;
        private readonly string _d3DDllName;

        private IntPtr _myD3DDll;
        private IntPtr _theirD3DDll;
        protected readonly IntPtr D3DDevicePtr;

        protected Form Form { get; private set; }

        public static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeLibrary(IntPtr hModule);
        }

        private List<IntPtr> _loadedLibraries = new List<IntPtr>();
        protected D3DDevice(Process targetProcess, string d3DDllName)
        {
            TargetProcess = targetProcess;
            _d3DDllName = d3DDllName;
            Form = new Form();
            LoadDll();
            InitD3D(out D3DDevicePtr);
        }
        /// <summary>
        /// initiializes d3d and sets device pointer.
        /// </summary>
        protected abstract void InitD3D(out IntPtr d3DDevicePtr);

        /// <summary>
        /// Cleanup ))
        /// </summary>
        protected abstract void CleanD3D();

        public abstract int BeginSceneVtableIndex { get; }
        public abstract int EndSceneVtableIndex { get; }
        public abstract int PresentVtableIndex { get; }

        private void LoadDll()
        {
            _myD3DDll = LoadLibrary(_d3DDllName);
            if (_myD3DDll == IntPtr.Zero)
                throw new Exception(String.Format("Could not load {0}", _d3DDllName));

            _theirD3DDll = TargetProcess.Modules.Cast<ProcessModule>().First(m => m.ModuleName == _d3DDllName).BaseAddress;
        }

        protected IntPtr LoadLibrary(string library)
        {
            // Attempt to grab the module handle if its loaded already.
            var ret = NativeMethods.GetModuleHandle(library);
            if (ret == IntPtr.Zero)
            {
                // Load the lib manually if its not, storing it in a list so we can free it later.
                ret = NativeMethods.LoadLibrary(library);
                _loadedLibraries.Add(ret);
            }
            return ret;
        }


        protected unsafe IntPtr GetVTableFuncAddress(IntPtr obj, int funcIndex)
        {
            var pointer = *(IntPtr*)((void*)obj);
            return *(IntPtr*)((void*)((int)pointer + funcIndex * 4));
        }

        public unsafe IntPtr GetDeviceVTableFuncAbsoluteAddress(int funcIndex)
        {
            var pointer = *(IntPtr*)((void*)D3DDevicePtr); //Expect the unexpected
            pointer = *(IntPtr*)((void*)((int)pointer + funcIndex * 4));
            var offset = IntPtr.Subtract(pointer, _myD3DDll.ToInt32());
            return IntPtr.Add(_theirD3DDll, offset.ToInt32());
        }

        protected T GetDelegate<T>(IntPtr address) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer(address, typeof(T)) as T;
        }

        #region IDisposable Pattern Implementation
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CleanD3D();

                    foreach (var loadedLibrary in _loadedLibraries)
                    {
                        NativeMethods.FreeLibrary(loadedLibrary);
                    }
                }
                _disposed = true;
            }
        }

        ~D3DDevice()
        {
            Dispose(false);
        }
        #endregion

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        protected delegate void VTableFuncDelegate(IntPtr instance);
    }
}
