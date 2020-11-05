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

        public TextAnimationBase GetAnimator<T>() where T : TextAnimationBase
        {
            if (_animations.ContainsKey(typeof(T)))
            {
                return _animations[typeof(T)];
            }
            else
            {
                Debug.LogError($"Type: {typeof(T).Name} is not in the factory.");
                return null;
            }
        }
    }
}
