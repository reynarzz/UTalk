﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public abstract class TextAnimationBase
    {
        private TextControl _textControl;
        private TextPage _textPage;

        private List<int> _charIndexesToAnimate;
        protected List<int> CharIndexesToAnimate => _charIndexesToAnimate;

        protected TextControl TextControl => _textControl;
        protected TextPage TextPage => _textPage;

        public TextAnimationBase()
        {
            _charIndexesToAnimate = new List<int>();
        }

        public abstract void Update();

        public virtual void Init(TextControl textControl, TextPage page)
        {
            _textControl = textControl;
            _charIndexesToAnimate.Clear();
        }

        public void AddChar(int charIndex) 
        {
            _charIndexesToAnimate.Add(charIndex);
        }

        public void AddChars(List<int> charsIndex) 
        {
            //copy the list here.
            _charIndexesToAnimate = charsIndex.ToList();
        }
    }
}
