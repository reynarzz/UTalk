using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    [Serializable]
    /// <summary>Serializable color</summary>
    public struct SColor32
    {
        private int _r;
        private int _g;
        private int _b;
        private int _a;

        public int r => _r;
        public int g => _g;
        public int b => _b;
        public int a => _a;
      
        public SColor32(int r, int g, int b, int a)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public SColor32(Color color)
        {
            _r = Mathf.RoundToInt(color.r * 255);
            _g = Mathf.RoundToInt(color.g * 255);
            _b = Mathf.RoundToInt(color.b * 255);
            _a = Mathf.RoundToInt(color.a * 255);
        }

        public SColor32(Color32 color)
        {
            _r = color.r;  
            _g = color.g;
            _b = color.b;
            _a = color.a;
        }

        public static implicit operator SColor32(Color color)
        {
            return new SColor32(color);
        }

        public static implicit operator Color(SColor32 color)
        {
            return new Color(1f / color.r, 1f / color.g, 1f / color.b, 1f / color.a);
        }

        public static implicit operator SColor32(Color32 color)
        {
            return new SColor32(color);
        }

        public static implicit operator Color32(SColor32 color)
        {
            return new Color32((byte)color.r, (byte)color.g, (byte)color.b, (byte)color.a);
        }
    }
}