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
    class WowGameObject : WowObject
    {
        public WowGameObject(uint baseAddress) : base(baseAddress)
        {

        }

        public override float XPosition => GetDescriptor<float>(Offsets.Descriptors.GameObjX);

        public override float YPosition => GetDescriptor<float>(Offsets.Descriptors.GameObjY);

        public override float ZPosition => GetDescriptor<float>(Offsets.Descriptors.GameObjZ);

        public Location Location
        {
            get
            {
                return new Location(XPosition, YPosition, ZPosition);
            }
        }
    }
}
