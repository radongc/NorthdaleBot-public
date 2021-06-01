using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using NorthdaleBotWpf.Game;
using NorthdaleBotWpf.Objects;

namespace NorthdaleBotWpf.UI
{
    public class UIRefresh
    {
        private static Dictionary<string, TextBlock> UIElementDict = new Dictionary<string, TextBlock>();

        public static void AddUIElement(string identifier, TextBlock block)
        {
            UIElementDict.Add(identifier, block);
        }

        public static void StartUITimer()
        {
            Timer uiTimer = new Timer();

            uiTimer.Interval = 1000;
            uiTimer.Elapsed += new ElapsedEventHandler(UITick);
            uiTimer.Enabled = true;
        }

        public static void UITick(object sender, ElapsedEventArgs e) // TODO efficiency changes
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                UIElementDict["charName"].Text = ObjectManager.PlayerObject.Name;
                UIElementDict["charLvl"].Text = ObjectManager.PlayerObject.Level.ToString();
                UIElementDict["charHealth"].Text = $"{ObjectManager.PlayerObject.CurrentHealth}/{ObjectManager.PlayerObject.MaxHealth}";
                UIElementDict["charMana"].Text = $"{ObjectManager.PlayerObject.CurrentMana}/{ObjectManager.PlayerObject.MaxMana}";
                UIElementDict["charEnergy"].Text = ObjectManager.PlayerObject.Energy.ToString();
                UIElementDict["charRage"].Text = ObjectManager.PlayerObject.Rage.ToString();
                UIElementDict["charZone"].Text = ObjectManager.PlayerObject.RealZoneText;
                UIElementDict["charArea"].Text = ObjectManager.PlayerObject.MinimapZoneText;
                UIElementDict["charX"].Text = ObjectManager.PlayerObject.XPosition.ToString();
                UIElementDict["charY"].Text = ObjectManager.PlayerObject.YPosition.ToString();
                UIElementDict["charZ"].Text = ObjectManager.PlayerObject.ZPosition.ToString();

                WowUnit playerTarget = (WowUnit)ObjectManager.GetObjectByGuid(ObjectManager.PlayerObject.TargetGuid);

                if (playerTarget != null)
                {
                    WowUnit targetSummoner = (WowUnit)ObjectManager.GetObjectByGuid(playerTarget.SummonedByGuid);

                    UIElementDict["targetName"].Text = playerTarget.Name;
                    UIElementDict["targetLvl"].Text = playerTarget.Level.ToString();
                    UIElementDict["targetHealth"].Text = $"{playerTarget.CurrentHealth}/{playerTarget.MaxHealth}";
                    UIElementDict["targetMana"].Text = $"{playerTarget.CurrentMana}/{playerTarget.MaxMana}";
                    UIElementDict["targetNpcId"].Text = playerTarget.NpcID.ToString();
                    UIElementDict["targetFactionId"].Text = playerTarget.FactionID.ToString();
                    UIElementDict["targetMaster"].Text = targetSummoner == null ? string.Empty : targetSummoner.Name;
                    UIElementDict["targetX"].Text = playerTarget.XPosition.ToString();
                    UIElementDict["targetY"].Text = playerTarget.YPosition.ToString();
                    UIElementDict["targetZ"].Text = playerTarget.ZPosition.ToString();
                }
                else
                {
                    UIElementDict["targetName"].Text = string.Empty;
                    UIElementDict["targetLvl"].Text = string.Empty;
                    UIElementDict["targetHealth"].Text = string.Empty;
                    UIElementDict["targetMana"].Text = string.Empty;
                    UIElementDict["targetNpcId"].Text = string.Empty;
                    UIElementDict["targetFactionId"].Text = string.Empty;
                    UIElementDict["targetMaster"].Text = string.Empty;
                    UIElementDict["targetX"].Text = string.Empty;
                    UIElementDict["targetY"].Text = string.Empty;
                    UIElementDict["targetZ"].Text = string.Empty;
                }
            });
        }
    }
}
