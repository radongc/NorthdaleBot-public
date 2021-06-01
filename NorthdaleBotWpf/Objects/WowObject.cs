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
    class WowObject
    {
        protected uint baseAddress;

        public WowObject(uint baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        public uint BaseAddress
        {
            get => baseAddress;
            set => baseAddress = value;
        }

        public int Type => ReadRelative<int>((int)Offsets.ObjectManager.ObjType);

        public virtual ulong Guid => ReadRelative<ulong>((int)Offsets.ObjectManager.CurObjGuid);

        public virtual float XPosition { get; }
        public virtual float YPosition { get; }
        public virtual float ZPosition { get; }

        public T GetDescriptor<T>(int descriptor) where T : struct
        {
            var ptr = ObjectManager.Reader.Read<uint>(IntPtr.Add((IntPtr)BaseAddress, Offsets.ObjectManager.DescriptorOffset));

            return ObjectManager.Reader.Read<T>(new IntPtr(ptr + descriptor));
        }

        public T ReadRelative<T>(int offset) where T : struct
        {
            return ObjectManager.Reader.Read<T>((IntPtr)BaseAddress + offset);
        }

        public T ReadRelativeProcess<T>(int offset) where T : struct
        {
            return ObjectManager.Reader.Read<T>(ObjectManager.GameProcess.BaseAddress() + offset);
        }
    }
}
