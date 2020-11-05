using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public class TextAnimationControl 
    {
        private TextPage _textPage;
        private TextControl _textControl;

        private TextAnimatorFactory _animatorsFactory;

        private List<TextAnimationBase> _animators;

        public TextAnimationControl()
        {
            _animatorsFactory = new TextAnimatorFactory();
        }

        public void Init(TextControl textControl, TextPage textPage)
        {
            _textControl = textControl;
            _textPage = textPage;

            _animators.Clear();
        }

        public void HighlightedChar(int charIndex, Highlight highlight)
        {
            switch (highlight.Type)
            {
                case HighlightAnimation.None:
                    break;
                case HighlightAnimation.Sine:
                    var sine = GetAnimator<SineCharAnimation>();
                    sine.AddChar(charIndex);
                    break;
            }
        }

        public void NormalChar(int charIndex)
        {
            switch (_textPage.WriteType)
            {
                case WriteType.Instant:
                    break;
                case WriteType.CharByChar:
                    CharByCharSetUp();
                    break;
            }

        }

        private void CharByCharSetUp()
        {
            //_textPage.CharByCharInfo

        }

        public void Update()
        {
            for (int i = 0; i < _animators.Count; i++)
            {
                _animators[i].Update();
            }
        }

        private TextAnimationBase GetAnimator<T>() where T: TextAnimationBase
        {
            var animator = _animatorsFactory.GetAnimator<T>(_textControl, _textPage);

            if (!_animators.Contains(animator))
            {
                _animators.Add(animator);
            }

            return animator;
        }
    }
}
