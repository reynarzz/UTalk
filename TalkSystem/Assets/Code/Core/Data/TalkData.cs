//MIT License

//Copyright (c) 2020 Reynardo Perez (Reynarz)

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public enum WriteType
    {
        CharByChar,
        Fade,
        CharByCharAnimated
    }

    public enum HighlightAnimation
    {
        None,
        Sine
    }

    public enum Language
    {
        English,
        French,
        Spanish
        //Others...
    }

    [Serializable]
    public struct WordEvent
    {
        [SerializeField] private int _wordIndex;
        public int WordIndex => _wordIndex;

        public WordEvent(int wordIndex)
        {
            _wordIndex = wordIndex;
        }
    }

    [Serializable]
    public struct TextPage
    {
        [SerializeField, TextArea] private string _pageText;
        [SerializeField, HideInInspector] private Sprite _sprite; //later
        [SerializeField] private SDictionary<int, Highlight> _highlight;
        [SerializeField] private WordEvent _wordEvent;

        [Header("Write Styles")]
        [SerializeField] private CharByCharInfo _charByChar;

        public string Text => _pageText;
        public Sprite Sprite => _sprite;
        public SDictionary<int, Highlight> Highlight => _highlight;
        public WordEvent Event => _wordEvent;

        #region WriteStyleInfo
        public CharByCharInfo CharByCharInfo => _charByChar;
        #endregion

        public TextPage(string text, Sprite sprite, WordEvent wEvent)
        {
            _pageText = text;
            _sprite = sprite;
            _wordEvent = wEvent;

            _highlight = new SDictionary<int, Highlight>();

            _charByChar = default;
        }

        public TextPage(string text, Sprite sprite, WordEvent wEvent, SDictionary<int, Highlight> highlights)
        {
            _pageText = text;
            _sprite = sprite;
            _wordEvent = wEvent;
            _highlight = highlights;

            _charByChar = default;
        }

        public TextPage(string text, SDictionary<int, Highlight> highlights)
        {
            _pageText = text;
            _sprite = default;
            _wordEvent = default;
            _highlight = highlights;

            _charByChar = default;
        }

        public static bool operator ==(TextPage a, TextPage b)
        {
            return a.Text == b.Text && a.Highlight == b.Highlight && a.Sprite == b.Sprite;
        }

        public static bool operator !=(TextPage a, TextPage b)
        {
            return a.Text != b.Text || a.Highlight != b.Highlight || a.Sprite != b.Sprite;
        }
    }

    //TODO: Save highlighted words by index
    [Serializable]
    public struct Highlight
    {
        [SerializeField] private int _wordIndex;
        [SerializeField] private int _wordStartCharIndex;
        [SerializeField] private int _highlightLength;
        [SerializeField] private Color32 _color;
        [SerializeField] private HighlightAnimation _animationType;

        public int WordIndex => _wordIndex;
        public int CharIndex => _wordStartCharIndex;
        public int HighlighLength => _highlightLength;

        public Color32 Color => _color;
        public HighlightAnimation Type => _animationType;

        public Highlight(int wordIndex, int startChar, int highlightLength, Color32 color) : this(wordIndex, startChar, highlightLength, color, default) { }

        public Highlight(int wordIndex, int startChar, int highlightLength, Color32 color, HighlightAnimation type)
        {
            _wordIndex = wordIndex;
            _wordStartCharIndex = startChar;
            _highlightLength = highlightLength;

            _color = color;
            _animationType = type;
        }

        public static bool operator ==(Highlight a, Highlight b)
        {
            return a._wordIndex == b._wordIndex && a._wordStartCharIndex == b._wordStartCharIndex &&
                   a._highlightLength == b._highlightLength && a._animationType == b._animationType;
        }

        public static bool operator !=(Highlight a, Highlight b)
        {
            return a._wordIndex != b._wordIndex || a._wordStartCharIndex != b._wordStartCharIndex ||
                   a._highlightLength != b._highlightLength || a._animationType != b._animationType;
        }
    }

    [Serializable]
    public class TalkData
    {
        [SerializeField, HideInInspector] private string _talkName;

        [SerializeField, HideInInspector] private Language _language;
        [SerializeField] private List<TextPage> _pages;

        public string TalkName { get => _talkName; set => _talkName = value; }
        public Language Language { get => _language; set => _language = value; }
        public int PagesCount => _pages.Count;

        public TalkData(List<TextPage> pages)
        {
            _pages = pages;
        }

        public TalkData()
        {
            _pages = new List<TextPage>();
        }

        public TextPage GetPage(int pageIndex)
        {
            return _pages.ElementAtOrDefault(pageIndex);
        }

        public void AddPage(TextPage page)
        {
            _pages.Add(page);
        }

        public static implicit operator bool(TalkData a)
        {
            return a != null;
        }
    }
}
