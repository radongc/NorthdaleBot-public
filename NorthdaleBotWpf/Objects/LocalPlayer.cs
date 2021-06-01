using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using NorthdaleBotWpf.Game;
using NorthdaleBotWpf.Helpful;
using NorthdaleBotWpf.Constants;
using NorthdaleBotWpf.Hook;

namespace NorthdaleBotWpf.Objects
{
    class LocalPlayer : WowUnit
    {
        public LocalPlayer(uint baseAddress) : base(baseAddress)
        {

        }

        public string RealZoneText
        {
            get
            {
                var ptr1 = ObjectManager.Reader.Read<IntPtr>(Offsets.Player.RealZoneText);

                var ptr2 = ObjectManager.Reader.ReadString(ptr1, Encoding.UTF8);

                return ptr2;
            }
        }

        public string MinimapZoneText
        {
            get
            {
                var ptr1 = ObjectManager.Reader.Read<IntPtr>(Offsets.Player.MinimapZoneText);

                var ptr2 = ObjectManager.Reader.ReadString(ptr1, Encoding.UTF8);

                return ptr2;
            }
        }

        public float Facing
        {
            get
            {
                return ReadRelative<float>(Offsets.Player.Facing);
            }
        }

        // Methods

        public bool IsFacing(Location Loc)
        {
            float f = (float)Math.Atan2(Loc.Y - Location.Y, Loc.X - Location.X);

            if (f < 0.0f)
            {
                f = f + MathUtility.PI * 2.0f;
            }
            else
            {
                if (f > MathUtility.PI * 2)
                {
                    f = f - MathUtility.PI * 2.0f;
                }
            }

            return f == Facing ? true : false;
        }

        public float RequiredFacing(Location posToFace)
        {
            float f = (float)Math.Atan2(posToFace.Y - Location.Y, posToFace.X - Location.X);

            if (f < 0.0f)
            {
                f = f + MathUtility.PI * 2.0f;
            }
            else
            {
                if (f > MathUtility.PI * 2)
                {
                    f = f - MathUtility.PI * 2.0f;
                }
            }

            return f;
        }

        public void Face(Location posToFace)
        {
            GameCalls.SetFacing(RequiredFacing(posToFace)); // math of where to face is not working propery but the function itself is.
            GameCalls.SendMovementUpdate(0xDA, (uint)Environment.TickCount);
        }

        public void MoveForward()
        {
            if (!GameCalls.MovementContainsFlag((uint)Offsets.MovementFlags.Forward))
            {
                //GameCalls.SetControlBit(Offsets.ControlBits.Back, 0);
                GameCalls.SetControlBit(Offsets.ControlBits.Front, 1);
            }
        }

        public void MoveBackward()
        {
            if (!GameCalls.MovementContainsFlag((uint)Offsets.MovementFlags.Back))
            {
                GameCalls.SetControlBit(Offsets.ControlBits.Back, 1);
                //GameCalls.SetControlBit(Offsets.ControlBits.Front, 0);
            }
        }

        public void MoveRight()
        {
            if (!GameCalls.MovementContainsFlag((uint)Offsets.MovementFlags.TurnRight))
            {
                //GameCalls.SetControlBit(Offsets.ControlBits.Left, 0);
                GameCalls.SetControlBit(Offsets.ControlBits.Right, 1);
            }
        }

        public void MoveLeft()
        {
            if (!GameCalls.MovementContainsFlag((uint)Offsets.MovementFlags.TurnRight))
            {
                GameCalls.SetControlBit(Offsets.ControlBits.Left, 1);
                //GameCalls.SetControlBit(Offsets.ControlBits.Right, 0);
            }
        }

        public void StopMoving()
        { 
            GameCalls.SetControlBit(Offsets.ControlBits.Back, 0);
            GameCalls.SetControlBit(Offsets.ControlBits.Front, 0);
            GameCalls.SetControlBit(Offsets.ControlBits.Left, 0);
            GameCalls.SetControlBit(Offsets.ControlBits.Right, 0);
        }

        public void CastSpellByName(string name) => GameCalls.DoString($"CastSpellByName('{name}')");

        public void TargetUnit(string unitName)
        {
            WowUnit potentialTarget = ObjectManager.GetUnitByName(unitName);

            GameCalls.SetTarget(potentialTarget.Guid);
        }

        public void TargetUnit(ulong unitGuid) => GameCalls.SetTarget(unitGuid);
    }
}
