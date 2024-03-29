﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace uTalk
{
    public abstract class TextAnimationBase
    {
        private List<int> _charIndexesToAnimate;
        private TextControl _textControl;
        private TextPage _textPage;

        protected TextControl TextControl => _textControl;
        protected TextPage TextPage => _textPage;

        protected int CharsToAnimateCount => _charIndexesToAnimate.Count;

        public TextAnimationBase()
        {
            _charIndexesToAnimate = new List<int>();
        }

        public abstract void Update();

        public virtual void Init(TextControl textControl, TextPage page)
        {
            _textControl = textControl;
            _textPage = page;
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

        protected int GetValidCharToAnimate(int index)
        {
            return _charIndexesToAnimate.ElementAtOrDefault(index);
        }

        public abstract void OnExitPage();
    }
}
