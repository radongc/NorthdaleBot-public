/* This was written before I had any good knowledge of reverse engineering or memory editing etc. so take with a grain of salt! */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NorthdaleBotWpf.Game;
using NorthdaleBotWpf.UI;
using NorthdaleBotWpf.Constants;
using NorthdaleBotWpf.Hook;
using NorthdaleBotWpf.Objects;

namespace NorthdaleBotWpf
{
    /// <summary>
    /// MainWindow Console class, used to route console output into a RTB.
    /// </summary>
    public static class Console
    {
        public static FlowDocument OutputDocument;
        public static Paragraph OutputParagraph;

        public static void Init(FlowDocument outputDocument, Paragraph outputParagraph)
        {
            OutputDocument = outputDocument;
            OutputParagraph = outputParagraph;
        }

        public static void WriteLine(string value)
        {
            Debug.WriteLine(value);

            if (OutputDocument != null && OutputParagraph != null)
            {
                OutputParagraph.Inlines.Add(new Run($"{value}\n"));
                OutputDocument.Blocks.Add(OutputParagraph);
            }
        }

        public static void WriteDebug(string value)
        {
            string content = $"[DEBUG] {value}";

            Debug.WriteLine(content);

            if (OutputDocument != null && OutputParagraph != null)
            {
                OutputParagraph.Inlines.Add(new Run($"{content}\n"));
                OutputDocument.Blocks.Add(OutputParagraph);
            }
        }

        public static void WriteError(string value)
        {
            string content = $"[ERROR] {value}";

            Debug.WriteLine(content);

            if (OutputDocument != null && OutputParagraph != null)
            {
                OutputParagraph.Inlines.Add(new Run($"{content}\n"));
                OutputDocument.Blocks.Add(OutputParagraph);
            }
        }

        public static void WriteWarning(string value)
        {
            string content = $"[WARNING] {value}";

            Debug.WriteLine(content);

            if (OutputDocument != null && OutputParagraph != null)
            {
                OutputParagraph.Inlines.Add(new Run($"{content}\n"));
                OutputDocument.Blocks.Add(OutputParagraph);
            }
        }

        public static void Clear()
        {
            if (OutputDocument != null && OutputParagraph != null)
            {
                OutputParagraph.Inlines.Clear();
                OutputDocument.Blocks.Clear();
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BotInit();
        }

        private void BotInit()
        {
            Console.Init(flowDocumentConsoleMain, paragraphConsoleMain);

            ObjectManager.OnPlayerFound = OnPlayerFoundCallback;

            ObjectManager.LoadAddresses();
        }

        private void ButtonClearConsole_Click(object sender, RoutedEventArgs e)
        {
            Console.Clear();
        }

        private void OnPlayerFoundCallback()
        {
            // Player
            UIRefresh.AddUIElement("charName", textBlockCharNameData);
            UIRefresh.AddUIElement("charLvl", textBlockCharLvlData);
            UIRefresh.AddUIElement("charHealth", textBlockCharHealthData);
            UIRefresh.AddUIElement("charMana", textBlockCharManaData);
            UIRefresh.AddUIElement("charEnergy", textBlockCharEnergyData);
            UIRefresh.AddUIElement("charRage", textBlockCharRageData);
            UIRefresh.AddUIElement("charZone", textBlockCharZoneNameData);
            UIRefresh.AddUIElement("charArea", textBlockCharAreaNameData);
            UIRefresh.AddUIElement("charX", textBlockCharXPosData);
            UIRefresh.AddUIElement("charY", textBlockCharYPosData);
            UIRefresh.AddUIElement("charZ", textBlockCharZPosData);

            // Target
            UIRefresh.AddUIElement("targetName", textBlockTargetNameData);
            UIRefresh.AddUIElement("targetLvl", textBlockTargetLvlData);
            UIRefresh.AddUIElement("targetHealth", textBlockTargetHealthData);
            UIRefresh.AddUIElement("targetMana", textBlockTargetManaData);
            UIRefresh.AddUIElement("targetNpcId", textBlockTargetNpcIdData);
            UIRefresh.AddUIElement("targetFactionId", textBlockTargetFactionIdData);
            UIRefresh.AddUIElement("targetMaster", textBlockTargetSummonedByData);
            UIRefresh.AddUIElement("targetX", textBlockTargetXPosData);
            UIRefresh.AddUIElement("targetY", textBlockTargetYPosData);
            UIRefresh.AddUIElement("targetZ", textBlockTargetZPosData);


            UIRefresh.StartUITimer();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TestBot.SelectTarget();
        }
    }
}
