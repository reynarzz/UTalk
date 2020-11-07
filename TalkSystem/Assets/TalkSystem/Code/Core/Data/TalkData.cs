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
    public struct TalkInfo
    {
        [SerializeField] private string _talkName;
        [SerializeField] private string _groupName;
        [SerializeField] private string _subGroupName;
        [SerializeField] private Language _language;

        public string GroupName => _groupName;
        public string SubGroupName => _subGroupName;
        public string TalkName => _talkName;
        public Language Language => _language;

        public TalkInfo(string groupName, string subGroupName, string talkName, Language language)
        {
            _groupName = groupName;
            _subGroupName = subGroupName;
            _talkName = talkName;

            _language = language;
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

        public static bool operator ==(CharByCharInfo a, CharByCharInfo b)
        {
            return a._animationType == b._animationType &&
                   a._fastWriteSpeed == b._fastWriteSpeed &&
                   a._normalWriteSpeed == b._normalWriteSpeed &&
                   a._offset == b._offset &&
                   a._offsetType == b._offsetType;
        }

        public static bool operator !=(CharByCharInfo a, CharByCharInfo b)
        {
            return a._animationType != b._animationType ||
                  a._fastWriteSpeed != b._fastWriteSpeed ||
                  a._normalWriteSpeed != b._normalWriteSpeed ||
                  a._offset != b._offset ||
                  a._offsetType != b._offsetType;
        }
    }

    [Serializable]
    public struct InstantInfo
    {
        [SerializeField] private TextAnimation _TextAnimation;
        public TextAnimation TextAnimation { get => _TextAnimation; set => _TextAnimation = value; }

        public static bool operator ==(InstantInfo a, InstantInfo b)
        {
            return a._TextAnimation == b._TextAnimation;
        }

        public static bool operator !=(InstantInfo a, InstantInfo b)
        {
            return a._TextAnimation != b._TextAnimation;
        }
    }

    [Serializable]
    public class TextPage
    {
        [SerializeField, TextArea] private string _pageText;
        [SerializeField] private string _talkerName;
        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private SDictionary<int, Highlight> _highlight;
        [SerializeField] private WordEvent _wordEvent;
        [SerializeField] private WriteType _writeType;

        [Header("Write Styles")]
        [SerializeField] private CharByCharInfo _charByChar;
        [SerializeField] private InstantInfo _instantInfo;

        //public struct PageInfo
        //{
        //    [SerializeField] private CharByCharInfo _charByChar;
        //    [SerializeField] private InstantInfo _instantInfo;


        //}

        public string Text { get => _pageText; set => _pageText = value; }
        public string TalkerName { get => _talkerName; set => _talkerName = value; }
        public SDictionary<int, Highlight> Highlight => _highlight;
        public WordEvent Event => _wordEvent;

        public WriteType WriteType { get => _writeType; set => _writeType = value; }

        #region WriteStyleInfo 
        public CharByCharInfo CharByCharInfo { get => _charByChar; set => _charByChar = value; }
        public InstantInfo InstantInfo { get => _instantInfo; set => _instantInfo = value; }
        public List<Sprite> Sprites => _sprites;
        #endregion

        public TextPage GetDeepCopy()
        {
            var copyPage = MemberwiseClone() as TextPage;

            copyPage._sprites = new List<Sprite>();
            copyPage._highlight = new SDictionary<int, Highlight>();

            for (int i = 0; i < _sprites.Count; i++)
            {
                copyPage._sprites.Add(_sprites[i]);
            }

            for (int i = 0; i < _highlight.Count; i++)
            {
                var key = _highlight.ElementAt(i).Key;

                copyPage._highlight.Add(key, _highlight[key]);
            }

            return copyPage;
        }

        public TextPage(string text, List<Sprite> sprites)
        {
            _pageText = text;
            _sprites = sprites;
            _wordEvent = default;

            _highlight = new SDictionary<int, Highlight>();

            _charByChar = default;
        }

        public TextPage(string text, List<Sprite> sprites, WordEvent wEvent, SDictionary<int, Highlight> highlights)
        {
            _pageText = text;
            _sprites = sprites;
            _wordEvent = wEvent;
            _highlight = highlights;

            _charByChar = default;
        }

        public TextPage(string text, SDictionary<int, Highlight> highlights)
        {
            _pageText = text;
            _sprites = new List<Sprite>();
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
        [SerializeField] private HighlightWriteSpeed _writeSpeed;
        [SerializeField] private float _normalSpeed;

        public int WordIndex => _wordIndex;
        public int StartLocalChar => _wordStartCharIndex;
        public int HighlighLength => _highlightLength;

        public Color32 Color => _color;
        public TextAnimation Type => _animationType;
        public HighlightWriteSpeed WriteSpeedType => _writeSpeed;
        public float NormalWriteSpeed => _normalSpeed;

        public Highlight(int wordIndex, int startChar, int highlightLength, Color32 color) : this(wordIndex, startChar, highlightLength, color, default, default, default) { }

        public Highlight(int wordIndex, int startChar, int highlightLength, Color32 color, TextAnimation type,
                         HighlightWriteSpeed writeSpeedType, float normalSpeed)
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

        public enum HighlightWriteSpeed
        {
            Default, Custom
        }
    }

    [Serializable]
    public class TalkData
    {
        [SerializeField, HideInInspector] private TalkInfo _talkInfo;
        [SerializeField] private string _subGroup;
        [SerializeField] private string _group;

        [SerializeField] private List<TextPage> _pages;

        public TalkInfo TalkInfo => _talkInfo;

        public int PagesCount => _pages.Count;

        public TalkData(TalkInfo talkInfo, List<TextPage> pages)
        {
            _talkInfo = talkInfo;
            _pages = pages;
        }

        public TalkData(TalkInfo talkInfo)
        {
            _talkInfo = talkInfo;

            _pages = new List<TextPage>();
        }

        public TalkData GetDeepCopy()
        {
            var copy = MemberwiseClone() as TalkData;

            copy._pages = new List<TextPage>();

            for (int i = 0; i < _pages.Count; i++)
            {
                copy._pages.Add(_pages[i].GetDeepCopy());
            }

            return copy;
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

        public void CreateEmptyPageWithLastPageOptions()
        {
            var lastPage = _pages.ElementAt(_pages.Count - 1);
            var newPage = new TextPage("", lastPage.Sprites.ToList());

            newPage.WriteType = lastPage.WriteType;

            switch (lastPage.WriteType)
            {
                case WriteType.Instant:
                    newPage.InstantInfo = lastPage.InstantInfo;
                    break;
                case WriteType.CharByChar:
                    newPage.CharByCharInfo = lastPage.CharByCharInfo;
                    break;
            }

            newPage.TalkerName = lastPage.TalkerName;
            _pages.Add(newPage);
        }

        public static implicit operator bool(TalkData a)
        {
            return a != null;
        }
    }
}
