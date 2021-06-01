using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthdaleBotWpf.Game
{
    class Spell
    {
        private static Lazy<Spell> _instance = new Lazy<Spell>(() => new Spell());

        private readonly IReadOnlyDictionary<string, uint[]> PlayerSpells;

        private Spell()
        {
            var tmpPlayerSpells = new Dictionary<string, uint[]>();
            const uint currentPlayerSpellPtr = 0x00B700F0;
            uint index = 0;
            while (index < 1024)
            {
                var currentSpellId = ObjectManager.Reader.Read<uint>((IntPtr)(currentPlayerSpellPtr + 4 * index));
                if (currentSpellId == 0) break;
                var entryPtr = ObjectManager.Reader.Read<uint>((IntPtr)((0x00C0D780 + 8) + currentSpellId * 4));

                var entrySpellId = ObjectManager.Reader.Read<uint>((IntPtr)entryPtr);
                var namePtr = ObjectManager.Reader.Read<uint>((IntPtr)(entryPtr + 0x1E0));
                var name = ObjectManager.Reader.ReadString((IntPtr)namePtr, Encoding.UTF8);

                Console.WriteLine(entrySpellId + " " + name);

                if (tmpPlayerSpells.ContainsKey(name))
                {
                    var tmpIds = new List<uint>();
                    tmpIds.AddRange(tmpPlayerSpells[name]);
                    tmpIds.Add(entrySpellId);
                    tmpPlayerSpells[name] = tmpIds.ToArray();
                }
                else
                {
                    uint[] ranks = { entrySpellId };
                    tmpPlayerSpells.Add(name, ranks);
                }
                index += 1;
            }
            PlayerSpells = tmpPlayerSpells;
        }

        public static Spell Instance => _instance.Value;

        public static void UpdateSpellbook()
        {
            _instance = new Lazy<Spell>(() => new Spell());
        }
    }
}
