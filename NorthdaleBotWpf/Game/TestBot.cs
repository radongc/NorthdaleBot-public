using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthdaleBotWpf.Constants;
using NorthdaleBotWpf.Hook;
using NorthdaleBotWpf.Objects;

namespace NorthdaleBotWpf.Game
{
    class TestBot
    {
        public static WowUnit CurrentTarget;

        public static LocalPlayer Player
        {
            get
            {
                return ObjectManager.PlayerObject;
            }
        }

        public static void SelectTarget()
        {
            WowUnit ingameTarget = (WowUnit)ObjectManager.GetObjectByGuid(Player.TargetGuid);

            if (ingameTarget != null)
            {
                CurrentTarget = ingameTarget;

                Attack();
            }
            else
            {
                WowUnit tempTarget = ObjectManager.GetUnitByName("Defias Thug", true);

                if (tempTarget != null)
                {
                    CurrentTarget = tempTarget;

                    GameCalls.SetTarget(tempTarget.Guid);

                    Attack();
                }
            }
        }

        public static async void Attack()
        {
            try
            {
                bool castedSpell = false;

                if (CurrentTarget != null)
                {
                    while (CurrentTarget.CurrentHealth > 1)
                    {
                        if (!Player.IsFacing(CurrentTarget.Location))
                        {
                            Player.Face(CurrentTarget.Location);
                        }

                        if (Player.Location.CalcDistance(CurrentTarget.Location) > 2.5f)
                        {
                            Player.MoveForward();

                            if (Player.Location.CalcDistance(CurrentTarget.Location) < 20f && !castedSpell)
                            {
                                Player.CastSpellByName("Charge");
                                castedSpell = true;
                            }
                        }
                        else if (Player.Location.CalcDistance(CurrentTarget.Location) <= 2.5f)
                        {
                            Player.StopMoving();

                            await Task.Delay(100);

                            GameCalls.OnRightClickUnit(CurrentTarget.BaseAddress, 0);

                            await Task.Run(() => Rotation());
                        }
                    }
                }
            }
            catch (Exception b)
            {
                Console.WriteError(b.ToString());
            }
        }

        public static async void Rotation()
        {
            try
            {
                while (CurrentTarget != null)
                {
                    if (CurrentTarget.CurrentHealth > 0)
                    {
                        if (Player.Rage >= 15)
                        {
                            Player.CastSpellByName("Heroic Strike");
                        }
                    }

                    await Task.Delay(2000);

                    GameCalls.OnRightClickUnit(CurrentTarget.BaseAddress, 1);

                    await Task.Delay(2000);

                    SelectTarget();
                }
            }
            catch (Exception b)
            {
                Console.WriteError(b.ToString());
            }
        }
    }
}
