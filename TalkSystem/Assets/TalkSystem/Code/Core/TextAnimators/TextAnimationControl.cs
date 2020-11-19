using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace uTalk
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
            _animators = new List<TextAnimationBase>();
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
                case TextAnimation.None:
                    break;
                case TextAnimation.Sine:
                    var sine = GetAnimator<SineCharAnimation>();
                    sine.AddChar(charIndex);
                    break;
            }

            NormalChar(charIndex);
        }

        public void NormalChar(int charIndex)
        {
            switch (_textPage.WriteType)
            {
                case WriteType.Instant:
                    IntantAnim(charIndex);

                    break;
                case WriteType.CharByChar:
                    CharByCharAnim(charIndex);
                    break;
            }
        }

        private void IntantAnim(int charIndex)
        {
            switch (_textPage.InstantInfo.TextAnimation)
            {
                case TextAnimation.None:
                    break;
                case TextAnimation.Sine:
                    var sine = GetAnimator<SineCharAnimation>();
                    sine.AddChar(charIndex);
                    break;
            }
        }

        private void CharByCharAnim(int charIndex)
        {
            switch (_textPage.CharByCharInfo.AnimationType)
            {
                case CharByCharInfo.CharByCharAnimation.None:

                    break;
                case CharByCharInfo.CharByCharAnimation.OffsetToPos:
                    var offset = GetAnimator<OffsetCharAnimation>();
                    offset.AddChar(charIndex);
                    break;
            }
        }

        public void Update()
        {
            for (int i = 0; i < _animators.Count; i++)
            {
                _animators[i].Update();
            }
        }

        private TextAnimationBase GetAnimator<T>() where T : TextAnimationBase
        {
            var animator = _animatorsFactory.GetAnimator<T>();

            if (!_animators.Contains(animator))
            {
                animator.Init(_textControl, _textPage);
                _animators.Add(animator);
            }

            return animator;
        }

        public void OnExitPage()
        {
            for (int i = 0; i < _animators.Count; i++)
            {
                _animators[i].OnExitPage();
            }
        }
    }
}
