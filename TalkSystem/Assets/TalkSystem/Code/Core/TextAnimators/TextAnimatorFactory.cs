using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public class TextAnimatorFactory 
    {
        private Dictionary<Type, TextAnimationBase> _animations;

        public TextAnimatorFactory()
        {
            _animations = new Dictionary<Type, TextAnimationBase>()
            {
                { typeof(OffsetCharAnimation), new OffsetCharAnimation() },
                { typeof(SineCharAnimation), new SineCharAnimation() },
            };
        }

        public TextAnimationBase GetAnimator<T>(TextControl textControl, TextPage textPage) where T : TextAnimationBase
        {
            if (_animations.ContainsKey(typeof(T)))
            {
                var animator = _animations[typeof(T)];
                animator.Init(textControl, textPage);
                return animator;
            }
            else
            {
                Debug.LogError($"Type: {typeof(T).Name} is not in the factory.");
                return null;
            }
        }
    }
}
