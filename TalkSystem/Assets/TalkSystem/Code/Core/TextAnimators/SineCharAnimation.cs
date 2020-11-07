using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public class SineCharAnimation : TextAnimationBase
    {
        private float _freq = 10;
        private float _amp = 0.09f;

        public override void Update()
        {
            if (CharsToAnimateCount > 0)
            {
                for (int i = 0; i < CharsToAnimateCount; i++)
                {
                    //this needs work, is not very consistent.
                    TextControl.OffsetChar(GetValidCharToAnimate(i), new Vector2(0, Mathf.Sin(i + Time.time * _freq) * _amp));
                }
            }
        }

        public override void OnExitPage() { }
    }
}