using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthdaleBotWpf.Constants
{
    internal static class Offsets
    {
        internal static class Player
        {
            internal static IntPtr Name = (IntPtr)0x827D88; // 1.12.1 - Player name (string)
            internal static IntPtr Class = (IntPtr)0xC27E81; // 1.12.1 - class id (byte)

            internal static IntPtr XCoord = (IntPtr)0x00C7B548; // 1.12.1 - x coordinate (readonly) float
            internal static IntPtr YCoord = (IntPtr)0x00C7B54C; // 1.12.1 - y coordinate (readonly) float
            internal static IntPtr ZCoord = (IntPtr)0x00C7B544; // 1.12.1 - z coordinate (readonly) float

            internal static IntPtr IsLooting = (IntPtr)0xB71B48; // 1.12.1 - is the player looting? bool
            internal static IntPtr IsInGame = (IntPtr)0x00B4B424; // 1.12.1 - is the player ingame? bool

            internal static int MovementStruct = 0x9A8; // 1.12.1 - for changing movement (add the LocalPlayer base pointer to this offset)
            internal static int Facing = 0x9C4; // 1.12.1 - where the player is facing (float)

            internal static IntPtr RealZoneText = (IntPtr)0xB4B404; // 1.12.1 - read as IntPtr then read as string
            internal static IntPtr ContinentText = (IntPtr)0x00C961A0; // 1.12.1
            internal static IntPtr MinimapZoneText = (IntPtr)0xB4DA28; // 1.12.1 - read as IntPtr then read as string
        }

        internal static class Misc
        {
            internal static IntPtr GameVersion = (IntPtr)0x00837C04; // 1.12.1 - game version (string)
            internal static IntPtr RealmName = (IntPtr)0x00C27FC1; // 1.12.1 - realm name (string)
        }

        internal static class ObjectManager
        {
            internal static IntPtr CurObjGuid = (IntPtr)0x30;
            internal static IntPtr ManagerBase = (IntPtr)0x00B41414; // 1.12.1 tested
            internal static IntPtr PlayerGuid = (IntPtr)0xc0; // 1.12.1 tested
            internal static IntPtr FirstObj = (IntPtr)0xac;
            internal static IntPtr NextObj = (IntPtr)0x3c;
            internal static IntPtr ObjType = (IntPtr)0x14;
            internal static int DescriptorOffset = 0x8;
        }

        internal static class PlayerObject
        {
            internal static IntPtr NameBase = (IntPtr)0xC0E230;
            internal static int NameBaseNextGuid = 0xc;
        }

        internal static class Unit
        {
            internal static int PosX = 0x9B8;
            internal static int PosY = 0x9BC;
            internal static int PosZ = 0x9BC + 4;
            internal static int AuraBase = 0xBC;
            internal static int DebuffBase = 0x13C;

            internal static int NameBase = 0xB30;
            internal static int IsCritterOffset = 24;
        }

        internal static class GameObject
        {
            internal static int PosX = 0x9B8;
            internal static int PosY = 0x9BC;
            internal static int PosZ = 0x9BC + 4;
            internal static int AuraBase = 0xBC;
            internal static int DebuffBase = 0x13C;

            internal static int NameBase = 0xB30;
            internal static int IsCritterOffset = 24;
        }

        internal static class Functions
        {
            internal static IntPtr FrameScript__Execute = (IntPtr)0x304CD0; // 1.12.1

            internal static IntPtr CGGameUI__Target = (IntPtr)0x493540;

            internal static IntPtr CGInputControl__GetActive = (IntPtr)0x005143E0;

            internal static IntPtr CGInputControl__SetControlBit = (IntPtr)0x00515090;

            internal static IntPtr OnRightClickObject = (IntPtr)0x005F8660;

            internal static IntPtr OnRightClickUnit = (IntPtr)0x60BEA0;

            internal static IntPtr SetFacing = (IntPtr)0x007C6F30;

            internal static IntPtr SendMovementPacket = (IntPtr)0x00600A30;
        }

        internal static class MovementFlags
        {
            internal static int None = 0x00000000;
            internal static int Forward = 0x00000001;
            internal static int Back = 0x00000002;
            internal static int TurnLeft = 0x00000010;
            internal static int TurnRight = 0x00000020;
            internal static int Stunned = 0x00001000;
            internal static int Swimming = 0x00200000;
        }

        internal static class ControlBits
        {
            internal static uint Front = 0x10;
            internal static uint Right = 0x200;
            internal static uint Left = 0x100;
            internal static uint Back = 0x20;
        }

        internal static class Descriptors
        {
            // UNIT
            internal static int SummonedByGuid = 0x30;

            internal static int NpcId = 0xE74;

            internal static int Health = 0x58;
            internal static int MaxHealth = 0x70;
            internal static int FactionId = 0x8C;
            internal static int Mana = 0x5C;
            internal static int MaxMana = 0x74;
            internal static int Rage = 0x60;
            internal static int MovementFlags = 0x9E8;
            internal static int Energy = 0x68;
            internal static int TargetGuid = 0x40;
            internal static int CorpseOwnedBy = 0x18;

            internal static int Level = 0x88;

            // PLAYEROBJ

            internal static int NextLevelXp = 0xB34;
            internal static int CurrentXp = 0xB30;

            // CORPSEOBJ

            internal static int CorpseX = 0x24;
            internal static int CorpseY = 0x28;
            internal static int CorpseZ = 0x2c;

            // GAMEOBJ

            internal static int GameObjX = 0x3C;
            internal static int GameObjY = 0x3C + 4;
            internal static int GameObjZ = 0x3C + 8;

            // DYNAMICOBJ

            internal static int DynamicObjX = 0x2C;
            internal static int DynamicObjY = 0x30;
            internal static int DynamicObjZ = 0x34;
        }
    }
}
