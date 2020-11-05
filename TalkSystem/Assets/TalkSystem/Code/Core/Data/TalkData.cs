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
using TMPro;
using UnityEngine;

namespace TalkSystem
{
    public enum WriteType
    {
        Instant,
        CharByChar
    }

    public enum TextAnimation
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
    public struct CharByCharInfo
    {
        [SerializeField] private float _normalWriteSpeed;
        [SerializeField] private float _fastWriteSpeed;
        [SerializeField] private OffsetStartPos _offsetType;
        [SerializeField] private CharByCharAnimation _animationType;
        [SerializeField] private float _offset;

        public enum CharByCharAnimation
        {
            None,
            OffsetToPos
        }

        public enum OffsetStartPos
        {
            Top,
            TopLeft,
            TopRight,

            Bottom,
            BottomLeft,
            BottomRight,
        }

        public float NormalWriteSpeed { get => _normalWriteSpeed; set => _normalWriteSpeed = value; }
        public float FastWriteSpeed { get => _fastWriteSpeed; set => _fastWriteSpeed = value; }
        public OffsetStartPos OffsetType { get => _offsetType; set => _offsetType = value; }
        public CharByCharAnimation AnimationType { get => _animationType; set => _animationType = value; }
        public float Offset { get => _offset; set => _offset = value; }
    }

    [Serializable]
    public struct InstantInfo
    {
        [SerializeField] private TextAnimation _TextAnimation;
        public TextAnimation TextAnimation { get => _TextAnimation; set => _TextAnimation = value; }
    }

    [Serializable]
    public class TextPage
    {
        [SerializeField, TextArea] private string _pageText;
        [SerializeField, HideInInspector] private Sprite _sprite; //later
        [SerializeField] private SDictionary<int, Highlight> _highlight;
        [SerializeField] private WordEvent _wordEvent;
        [SerializeField] private WriteType _writeType;

        [Header("Write Styles")]
        [SerializeField] private CharByCharInfo _charByChar;
        [SerializeField] private InstantInfo _instantInfo;

        public string Text { get => _pageText; set => _pageText = value; }
        public Sprite Sprite => _sprite;
        public SDictionary<int, Highlight> Highlight => _highlight;
        public WordEvent Event => _wordEvent;

        public WriteType WriteType { get => _writeType; set => _writeType = value; }

        #region WriteStyleInfo 
        public CharByCharInfo CharByCharInfo { get => _charByChar; set => _charByChar = value; }
        public InstantInfo InstantInfo { get => _instantInfo; set => _instantInfo = value; }
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

        //public static bool operator ==(TextPage a, TextPage b)
        //{
        //    return a.Text == b.Text && a.Highlight == b.Highlight && a.Sprite == b.Sprite;
        //}

        //public static bool operator !=(TextPage a, TextPage b)
        //{
        //    return a.Text != b.Text || a.Highlight != b.Highlight || a.Sprite != b.Sprite;
        //}
    }

    //TODO: Save highlighted words by index
    [Serializable]
    public struct Highlight
    {
        [SerializeField] private int _wordIndex;
        [SerializeField] private int _wordStartCharIndex;
        [SerializeField] private int _highlightLength;
        [SerializeField] private Color32 _color;
        [SerializeField] private TextAnimation _animationType;
        [SerializeField] private WriteSpeed _writeSpeed;
        [SerializeField] private float _normalSpeed;

        public int WordIndex => _wordIndex;
        public int StartLocalChar => _wordStartCharIndex;
        public int HighlighLength => _highlightLength;

        public Color32 Color => _color;
        public TextAnimation Type => _animationType;
        public WriteSpeed WriteSpeedType => _writeSpeed;
        public float NormalWriteSpeed => _normalSpeed;

        public Highlight(int wordIndex, int startChar, int highlightLength, Color32 color) : this(wordIndex, startChar, highlightLength, color, default, default, default) { }

        public Highlight(int wordIndex, int startChar, int highlightLength, Color32 color, TextAnimation type,
                         WriteSpeed writeSpeedType, float normalSpeed)
        {
            _wordIndex = wordIndex;
            _wordStartCharIndex = startChar;
            _highlightLength = highlightLength;
            _writeSpeed = writeSpeedType;

            _normalSpeed = normalSpeed;

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

        public override bool Equals(object obj)
        {
            return obj is Highlight highlight &&
                   _wordIndex == highlight._wordIndex &&
                   _wordStartCharIndex == highlight._wordStartCharIndex &&
                   _highlightLength == highlight._highlightLength &&
                   EqualityComparer<Color32>.Default.Equals(_color, highlight._color) &&
                   _animationType == highlight._animationType &&
                   WordIndex == highlight.WordIndex &&
                   StartLocalChar == highlight.StartLocalChar &&
                   HighlighLength == highlight.HighlighLength &&
                   EqualityComparer<Color32>.Default.Equals(Color, highlight.Color) &&
                   Type == highlight.Type;
        }

        public override int GetHashCode()
        {
            int hashCode = -829550546;
            hashCode = hashCode * -1521134295 + _wordIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + _wordStartCharIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + _highlightLength.GetHashCode();
            hashCode = hashCode * -1521134295 + _color.GetHashCode();
            hashCode = hashCode * -1521134295 + _animationType.GetHashCode();
            hashCode = hashCode * -1521134295 + WordIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + StartLocalChar.GetHashCode();
            hashCode = hashCode * -1521134295 + HighlighLength.GetHashCode();
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }

        public enum WriteSpeed
        {
            Default, Custom
        }
    }

    [Serializable]
    public class TalkData
    {
        [SerializeField, HideInInspector] private string _talkName;
        [SerializeField] private string _subGroup = "Default";


        [SerializeField, HideInInspector] private Language _language;
        [SerializeField] private List<TextPage> _pages;

        public string TalkName { get => _talkName; set => _talkName = value; }
        public string SubGroup { get => _subGroup; set => _subGroup = value; }
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

        public void DeletePage(int textPageIndex)
        {
            _pages.RemoveAt(textPageIndex);
        }

        public void CreateEmptyPage()
        {
            _pages.Add(new TextPage("", new SDictionary<int, Highlight>()));
        }

        public static implicit operator bool(TalkData a)
        {
            return a != null;
        }
    }
}
