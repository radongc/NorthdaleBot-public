using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthdaleBotWpf.Game;
using NorthdaleBotWpf.Constants;
using NorthdaleBotWpf.Helpful;

namespace NorthdaleBotWpf.Objects
{
    class WowUnit : WowObject
    {
        public WowUnit(uint baseAddress) : base(baseAddress)
        {

        }

        public string Name
        {
            get
            {
                if (Type == (int)Enums.ObjType.OT_PLAYER)
                {
                    var nameBase = ObjectManager.Reader.Read<IntPtr>(Offsets.PlayerObject.NameBase);
                    while (true)
                    {
                        var nextGuid =
                            ObjectManager.Reader.Read<ulong>(IntPtr.Add(nameBase, Offsets.PlayerObject.NameBaseNextGuid));
                        if (nextGuid == 0)
                            return "";
                        if (nextGuid != Guid)
                            nameBase = ObjectManager.Reader.Read<IntPtr>(nameBase);
                        else
                            break;
                    }

                    var nameFinal = IntPtr.Add(nameBase, 0x14);

                    return ObjectManager.Reader.ReadString(nameFinal, Encoding.UTF8);
                }
                else if (Type == (int)Enums.ObjType.OT_UNIT)
                {
                    var ptr1 = ReadRelative<IntPtr>(Offsets.Unit.NameBase);

                    if (ptr1 == IntPtr.Zero)
                        return "";

                    var ptr2 = ObjectManager.Reader.Read<IntPtr>(ptr1);

                    if (ptr2 == IntPtr.Zero)
                        return "";

                    return ObjectManager.Reader.ReadString(ptr2, Encoding.UTF8);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public ulong TargetGuid => GetDescriptor<ulong>(Offsets.Descriptors.TargetGuid);

        public int Level => GetDescriptor<int>(Offsets.Descriptors.Level);

        public int CurrentHealth => GetDescriptor<int>(Offsets.Descriptors.Health);

        public int MaxHealth => GetDescriptor<int>(Offsets.Descriptors.MaxHealth);

        public int CurrentMana => GetDescriptor<int>(Offsets.Descriptors.Mana);

        public int MaxMana => GetDescriptor<int>(Offsets.Descriptors.MaxMana);

        public int HealthPercent
        {
            get
            {
                double pct = (CurrentHealth / MaxHealth) * 100;
                return (int)Math.Round(pct);
            }
        }

        public uint MovementState
        {
            get
            {
                try
                {
                    if (BaseAddress == 0 || Guid == 0) return 0;
                    return ReadRelative<uint>(Offsets.Descriptors.MovementFlags);
                }
                catch (Exception) { return 0; }
            }
        }

        public override float XPosition => ReadRelative<float>(Offsets.Unit.PosX);

        public override float YPosition => ReadRelative<float>(Offsets.Unit.PosY);

        public override float ZPosition => ReadRelative<float>(Offsets.Unit.PosZ);

        public Location Location
        {
            get
            {
                return new Location(XPosition, YPosition, ZPosition);
            }
        }

        #region Player Specific

        public int Rage
        {
            get
            {
                if (Type != (int)Enums.ObjType.OT_PLAYER)
                {
                    return 0;
                }

                return GetDescriptor<int>(Offsets.Descriptors.Rage) / 10; // rage appears as 10 times the real value
            }
        }

        public int Energy
        {
            get
            {
                if (Type != (int)Enums.ObjType.OT_PLAYER)
                {
                    return 0;
                }

                return GetDescriptor<int>(Offsets.Descriptors.Energy);
            }
        }

        public int CurrentXp
        {
            get
            {
                if (Type != (int)Enums.ObjType.OT_PLAYER)
                {
                    return 0;
                }

                return GetDescriptor<int>(Offsets.Descriptors.CurrentXp);
            }
        }

        public int NextLevelXp
        {
            get
            {
                if (Type != (int)Enums.ObjType.OT_PLAYER)
                {
                    return 0;
                }

                return GetDescriptor<int>(Offsets.Descriptors.NextLevelXp);
            }
        }

        #endregion

        #region NPC Specific

        public int NpcID
        {
            get
            {
                if (Type != (int)Enums.ObjType.OT_UNIT)
                {
                    return 0;
                }

                return ReadRelative<int>(Offsets.Descriptors.NpcId);
            }
        }

        public int FactionID
        {
            get
            {
                if (Type != (int)Enums.ObjType.OT_UNIT)
                {
                    return 0;
                }

                return GetDescriptor<int>(Offsets.Descriptors.FactionId);
            }
        }

        public ulong SummonedByGuid
        {
            get
            {
                if (Type != (int)Enums.ObjType.OT_UNIT)
                {
                    return 0;
                }

                return GetDescriptor<ulong>(Offsets.Descriptors.SummonedByGuid);
            }
        }

        #endregion
    }
}
