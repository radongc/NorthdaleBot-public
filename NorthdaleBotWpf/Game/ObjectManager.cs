using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreyMagic;
using NorthdaleBotWpf.Constants;
using NorthdaleBotWpf.Helpful;
using NorthdaleBotWpf.Objects;
using NorthdaleBotWpf.Hook;

namespace NorthdaleBotWpf.Game
{
    static class ObjectManager
    {
        public static ExternalProcessReader Reader;
        public static InProcessMemoryReader MemReader;
        public static AsmHookTemp WowHook;

        public static Process GameProcess;

        public static LocalPlayer PlayerObject;

        public delegate void OnPlayerFoundDelegate();
        public static OnPlayerFoundDelegate OnPlayerFound;

        public static List<WowCorpse> CorpseList = new List<WowCorpse>();
        public static List<WowDynamic> DynamicList = new List<WowDynamic>();
        public static List<WowGameObject> GameObjectList = new List<WowGameObject>();
        public static List<WowUnit> UnitList = new List<WowUnit>();

        private static uint _objMgrBase;
        public static ulong LocalGuid { get; private set; }

        public static bool LoadAddresses()
        {
            try
            {
                Process wowProc = Process.GetProcessesByName("WoW").FirstOrDefault();

                if (wowProc != null)
                {
                    Reader = new ExternalProcessReader(wowProc);
                    MemReader = new InProcessMemoryReader(wowProc);
                    WowHook = new AsmHookTemp(wowProc);

                    GameProcess = wowProc;

                    _objMgrBase = Reader.Read<uint>(Offsets.ObjectManager.ManagerBase);
                    LocalGuid = Reader.Read<uint>(IntPtr.Add((IntPtr)_objMgrBase, (int)Offsets.ObjectManager.PlayerGuid));

                    Console.WriteDebug("Attached to WoW.\n");
                    Console.WriteDebug($"Local Guid: {LocalGuid}");

                    PopulateLists();

                    return true;
                }
                else
                {
                    Console.WriteDebug("Could not find WoW.");
                    return false;
                }
            }
            catch (Exception b)
            {
                Console.WriteError($"Error: {b.ToString()}");
            }

            return false;
        }

        public static void PopulateLists()
        {
            CorpseList.Clear();
            DynamicList.Clear();
            GameObjectList.Clear();
            UnitList.Clear();

            WowObject currentObject = new WowObject(Reader.Read<uint>((IntPtr)(_objMgrBase + (int)Offsets.ObjectManager.FirstObj)));

            Console.WriteDebug("Populating object lists...");

            while (currentObject.BaseAddress != 0 && currentObject.BaseAddress % 2 == 0)
            {
                if (currentObject.Type == (int)Enums.ObjType.OT_CORPSE)
                {
                    CorpseList.Add(new WowCorpse(currentObject.BaseAddress));
                }
                else if (currentObject.Type == (int)Enums.ObjType.OT_DYNOBJ)
                {
                    DynamicList.Add(new WowDynamic(currentObject.BaseAddress));
                }
                else if (currentObject.Type == (int)Enums.ObjType.OT_GAMEOBJ) // gameobject
                {
                    GameObjectList.Add(new WowGameObject(currentObject.BaseAddress));
                }
                else if (currentObject.Type == (int)Enums.ObjType.OT_UNIT || currentObject.Type == (int)Enums.ObjType.OT_PLAYER) // npc or player
                {
                    UnitList.Add(new WowUnit(currentObject.BaseAddress));

                    if (currentObject.Type == (int)Enums.ObjType.OT_PLAYER && currentObject.Guid == LocalGuid) // it's the localplayer.
                    {
                        PlayerObject = new LocalPlayer(currentObject.BaseAddress);

                        if (OnPlayerFound != null) // set in MainWindow.xaml.cs
                        {
                            WowHook.InstallHook();
                            Spell.UpdateSpellbook();
                            OnPlayerFound.Invoke();
                        }
                    } 
                }

                currentObject.BaseAddress = Reader.Read<uint>((IntPtr)(currentObject.BaseAddress + (int)Offsets.ObjectManager.NextObj)); // move to next object
            }
        }

        public static WowObject GetObjectByGuid(ulong guid)
        {
            WowObject currentObject = new WowObject(Reader.Read<uint>((IntPtr)(_objMgrBase + (int)Offsets.ObjectManager.FirstObj)));

            while (currentObject.BaseAddress != 0 && currentObject.BaseAddress % 2 == 0)
            {
                if (currentObject.Guid == guid)
                {
                    if (currentObject.Type == (int)Enums.ObjType.OT_CORPSE) // obj is corpse
                    {
                        return new WowCorpse(currentObject.BaseAddress);
                    }
                    else if (currentObject.Type == (int)Enums.ObjType.OT_DYNOBJ)
                    {
                        return new WowDynamic(currentObject.BaseAddress);
                    }
                    else if (currentObject.Type == (int)Enums.ObjType.OT_GAMEOBJ)
                    { 
                        return new WowGameObject(currentObject.BaseAddress);
                    }
                    else if (currentObject.Type == (int)Enums.ObjType.OT_UNIT || currentObject.Type == (int)Enums.ObjType.OT_PLAYER) // it's an npc or player
                    {
                        return new WowUnit(currentObject.BaseAddress);
                    }
                }
                currentObject.BaseAddress = Reader.Read<uint>((IntPtr)(currentObject.BaseAddress + (int)Offsets.ObjectManager.NextObj));
            }
            return null;
        }

        public static WowUnit GetUnitByName(string name, bool mustBeAlive = false)
        {
            WowObject currentObject = new WowObject(Reader.Read<uint>((IntPtr)(_objMgrBase + (int)Offsets.ObjectManager.FirstObj)));

            while (currentObject.BaseAddress != 0 && currentObject.BaseAddress % 2 == 0)
            {
                if (currentObject.Type == (int)Enums.ObjType.OT_UNIT || currentObject.Type == (int)Enums.ObjType.OT_PLAYER)
                {
                    WowUnit tempUnit = new WowUnit(currentObject.BaseAddress);

                    if (tempUnit.Name == name)
                    {
                        if (mustBeAlive)
                        {
                            if (tempUnit.CurrentHealth > 0)
                            {
                                return tempUnit;
                            }
                        }
                        else
                        {
                            return tempUnit;
                        }
                    }
                }
                currentObject.BaseAddress = Reader.Read<uint>((IntPtr)(currentObject.BaseAddress + (int)Offsets.ObjectManager.NextObj));
            }
            return null; // if it gets to here & doesnt return before the loop is over, it means it didnt find the object, so return null.
        }

        public static WowUnit GetUnitWithinRange(float range)
        {
            WowObject currentObject = new WowObject(Reader.Read<uint>((IntPtr)(_objMgrBase + (int)Offsets.ObjectManager.FirstObj)));

            while (currentObject.BaseAddress != 0 && currentObject.BaseAddress % 2 == 0)
            {
                if (currentObject.Type == (int)Enums.ObjType.OT_UNIT || currentObject.Type == (int)Enums.ObjType.OT_PLAYER)
                {
                    WowUnit tempUnit = new WowUnit(currentObject.BaseAddress);

                    if (PlayerObject.Location.CalcDistance(tempUnit.Location) <= range)
                    {
                        return tempUnit;
                    }
                }
                currentObject.BaseAddress = Reader.Read<uint>((IntPtr)(currentObject.BaseAddress + (int)Offsets.ObjectManager.NextObj));
            }
            return null;
        }
    }
}
