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
using System.Threading.Tasks;
using UnityEngine;

namespace Talk
{
    public enum WriteType
    {
        CharByChar,
        Fade,
        CharByCharAnimated
    }

    public enum HighlightType
    {
        None,
        VerticalShake
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
        [SerializeField] private string _word;
        public string Word => _word;

        public WordEvent(string word)
        {
            _word = word;
        }
    }

    [Serializable]
    public struct TextPage
    {
        [SerializeField, TextArea] private string _pageText;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Highlight[] _highlight;
        [SerializeField] private WordEvent _wordEvent;

        [Header("Write Styles")]
        [SerializeField] private CharByCharInfo _charByChar;

        public string Text => _pageText;
        public Sprite Sprite => _sprite;
        public Highlight[] Highlight => _highlight;
        public WordEvent Event => _wordEvent;

        public static TextPage Error => new TextPage("Invalid Page", null, default, Talk.Highlight.Error);

        #region WriteStyleInfo
        public CharByCharInfo CharByCharInfo => _charByChar;
        #endregion

        public TextPage(string text, Sprite sprite, WordEvent wEvent, params Highlight[] highlights)
        {
            _pageText = text;
            _sprite = sprite;
            _wordEvent = wEvent;
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

    //TODO:Save highlighted words by index
    [Serializable]
    public struct Highlight
    {
        [SerializeField] private string _words;
        [SerializeField] private Color32 _color;
        [SerializeField] private HighlightType _type;

        public string Word => _words;
        public Color32 Color => _color;
        public HighlightType Type => _type;

        public Highlight(string words, Color32 color, HighlightType type)
        {
            _words = words;
            _color = color;
            _type = type;
        }

        public static Highlight Error => new Highlight("Invalid Page", new Color32(255, 0, 0, 1), HighlightType.VerticalShake);

        public static bool operator==(Highlight a, Highlight b)
        {
            return a.Word == b.Word && a.Type == b.Type;
        }

        public static bool operator !=(Highlight a, Highlight b)
        {
            return a.Word != b.Word || a.Type != b.Type;
        }
    }

    [CreateAssetMenu]
    public class TalkAsset : ScriptableObject
    {
        [SerializeField, HideInInspector] private Language _language;
        [SerializeField] private TextPage[] _pages;

        public Language Language => _language;
        public int PagesCount => _pages.Length;

        public TextPage GetPage(int pageIndex)
        {
            var page = _pages.ElementAtOrDefault(pageIndex);

            if(page == default)
            {
                page = TextPage.Error;
            }

            return page;
        }
    }
}
