using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GreyMagic;
using NorthdaleBotWpf.Game;

namespace NorthdaleBotWpf.Hook
{
    class AsmHook
    {
        private readonly object _injectionLock = new object();

        //private IntPtr _injectionAddress;

        private ExternalProcessReader _extReader;
        private InProcessMemoryReader _intReader;

        public IntPtr PlayerPtr;

        public Process GameProcess { get; private set; }

        public bool Installed { get; private set; }

        public AsmHook(Process gameProcess)
        {
            _extReader = new ExternalProcessReader(gameProcess);
            _intReader = new InProcessMemoryReader(gameProcess);

            GameProcess = gameProcess;
            Installed = false;
        }

        public void Install()
        {
            if (GameProcess != null)
            {
                if (!Installed)
                {
                    bool error = false;

                    try
                    {
                        //_injectionAddress = _extReader.AllocateMemory(0x4);
                        PlayerPtr = _extReader.AllocateMemory(0x4);

                        //_extReader.Write(_injectionAddress, 0);
                        _extReader.Write<ulong>(PlayerPtr, ObjectManager.PlayerObject.BaseAddress);

                        Installed = true;
                    }
                    catch (Exception b)
                    {
                        Console.WriteError(b.ToString());
                        error = true;
                    }

                    if (!error)
                    {
                        Console.WriteDebug("Installed Asm hook successfully!");
                    }
                }
            }
        }

        public void Uninstall()
        {
            if (GameProcess != null)
            {
                if (Installed)
                {
                    //_extReader.FreeMemory(_injectionAddress);
                    _extReader.FreeMemory(PlayerPtr);
                }
            }
        }

        public void InjectAndExecute(IEnumerable<string> asm)
        {
            lock (_injectionLock)
            {
                if (Installed)
                {
                    // clear assembly
                    _extReader.Asm.Clear();

                    // load assembly
                    foreach (string asmLine in asm)
                    {
                        _extReader.Asm.AddLine(asmLine);
                    }

                    var injectionCodecave = _extReader.AllocateMemory(_extReader.Asm.Assemble().Length);

                    try
                    {
                        /* NOTES */
                        /* Crashes on 'charge' (refer to TestBot -> TestBot.cs) with code below, injecting to injectionCodecave instead of writing (what is
                         * actually supposed to be done, writing makes little sense) locks up program and does nothing... absolutely no clue why. */

                        //_extReader.Write(injectionCodecave, _extReader.Asm.Assemble());

                        //_extReader.Write(_injectionAddress, injectionCodecave);

                        _extReader.Asm.InjectAndExecute((uint)injectionCodecave);

                        //_extReader.Write(_injectionAddress, (int)injectionCodecave); // inject assembly

                        //_extReader.Asm.InjectAndExecute((uint)_injectionAddress);
                    }
                    catch (Exception b)
                    {
                        Console.WriteError(b.ToString());
                    }
                    finally
                    {
                        new Timer(state => _extReader.FreeMemory((IntPtr)state), injectionCodecave, 100, 0); // free later because freeing now causes crashing
                    }
                }
                else
                {
                    Console.WriteDebug("Cannot execute asm if hook is not installed!");
                }
            }
        }
    }
}
