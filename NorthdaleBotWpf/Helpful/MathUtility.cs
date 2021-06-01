using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthdaleBotWpf.Game;

namespace NorthdaleBotWpf.Helpful
{
    class MathUtility
    {
        public static float PI = (float)Math.PI;

        public static string FloatToHex32(float value)
        {
            byte[] tmpBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(tmpBytes);
            }
            string str = "";
            for (int i = 0; i < tmpBytes.Length; i++)
            {
                str += tmpBytes[i].ToString("X2");
            }
            return str;
        }
    }
}
