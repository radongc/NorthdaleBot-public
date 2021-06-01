using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthdaleBotWpf.Constants;
using NorthdaleBotWpf.Game;
using NorthdaleBotWpf.Helpful;

namespace NorthdaleBotWpf.Hook
{
    class GameCalls
    {
        private static readonly object _callLock = new object(); // locking a method prevents it from running multiple times simultaneously

        public static void DoString(string command)
        {
            lock (_callLock)
            {
                IntPtr doStringArgCodecave = ObjectManager.Reader.AllocateMemory(Encoding.UTF8.GetBytes(command).Length + 1);

                ObjectManager.Reader.WriteBytes(doStringArgCodecave, Encoding.UTF8.GetBytes(command));

                string[] asm = new string[]
                {
                    "mov edx, " + doStringArgCodecave,
                    "mov ecx, " + doStringArgCodecave,
                    "call " + ((uint) Offsets.Functions.FrameScript__Execute + (uint)ObjectManager.GameProcess.MainModule.BaseAddress.ToInt32()),  // Lua_DoString   
                    "retn",
                };

                ObjectManager.WowHook.InjectAndExecute(asm);

                ObjectManager.Reader.FreeMemory(doStringArgCodecave);
            }
        }

        public static void SetTarget(ulong guid)
        {
            lock (_callLock)
            {
                byte[] guidBytes = BitConverter.GetBytes(guid);

                string[] asm = new string[]
                {
                    "push " + BitConverter.ToInt32(guidBytes, 4),
                    "push " + BitConverter.ToInt32(guidBytes, 0),
                    "call " + (uint)Offsets.Functions.CGGameUI__Target, // Takes a guid, sets target to unit with specified guid
                    "retn",
                };

                ObjectManager.WowHook.InjectAndExecute(asm);
            }
        }

        public static void SetControlBit(uint bit, int state)
        {
            lock (_callLock)
            {
                string[] asm = new string[]
                {
                    "call " + (uint)Offsets.Functions.CGInputControl__GetActive,
                    "mov ECX, EAX",
                    "push 0" ,
                    "push " + (uint)Environment.TickCount,
                    "push " + (int)state,
                    "push " + (uint)bit,
                    "call " + (uint)Offsets.Functions.CGInputControl__SetControlBit,
                    "retn"
                };

                ObjectManager.WowHook.InjectAndExecute(asm);
            }
        }

        public static void SendMovementUpdate(uint OpCode, uint timeStamp)
        {
            lock (_callLock)
            {
                string[] asm = new string[]
                {
                    "mov ECX, [" + ObjectManager.WowHook.PlayerPtr + "]",
                    "push 0",
                    "push 0",
                    "push " + (uint)OpCode,
                    "push " + (uint)timeStamp,
                    "call " + (uint)Offsets.Functions.SendMovementPacket, // Send Packet
                    "retn",
                };

                ObjectManager.WowHook.InjectAndExecute(asm);
            }
        }

        public static void SetFacing(float facing)
        {
            lock (_callLock)
            {
                string[] asm = new string[]
                {
                    "mov ECX, [" + ObjectManager.WowHook.PlayerPtr + "]",
                    "add ECX, " + (uint)Offsets.Player.MovementStruct, // add the movement struct to the player ptr
                    "push 0x" + MathUtility.FloatToHex32(facing),
                    "call " + (uint)Offsets.Functions.SetFacing,
                    "retn"
                };

                ObjectManager.WowHook.InjectAndExecute(asm);
            }
        }

        public static void OnRightClickObject(uint objPtr, int autoLoot)
        {
            lock (_callLock)
            {
                string[] asm = new string[]
                {
                    "push " + autoLoot,
                    "mov ecx, " + (uint)objPtr,
                    "call " + (uint)Offsets.Functions.OnRightClickObject,
                    "retn",
                };

                ObjectManager.WowHook.InjectAndExecute(asm);
            }
        }

        public static void OnRightClickUnit(uint unitPtr, int autoLoot)
        {
            lock (_callLock)
            {
                string[] asm = new string[]
                {
                    "push " + autoLoot,
                    "mov ecx, " + (uint)unitPtr,
                    "call " + (uint)Offsets.Functions.OnRightClickUnit,
                    "retn",
                };

                ObjectManager.WowHook.InjectAndExecute(asm);
            }   
        }

        public static bool MovementContainsFlag(uint flag)
        {
            return (ObjectManager.PlayerObject.MovementState & flag) == flag ? true : false;
        }
    }
}
